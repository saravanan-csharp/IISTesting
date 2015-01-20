using Microsoft.Web.Administration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;

namespace IISApplicationLibrary
{
    public class IISLibrary
    {
        string GetUnusedPortNo()
        {
            int PortStartIndex = 1000;
            int PortEndIndex = 2000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            Thread.Sleep(1000);
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            int unusedPort = 0;

            for (int port = PortStartIndex; port < PortEndIndex; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort.ToString();
        }

        public bool CreateWebsite(string websiteName, string virtualDirectoryPhysicalPath)
        {

            string applicationPoolName = "DefaultAppPool";
            string virtualDirectoryPath = "/";
            string hostHeader = "";
            string applicationPath = "/";
            bool bFlag = false;
            using (ServerManager serverMgr = new ServerManager())
            {
                ApplicationPool appPool = serverMgr.ApplicationPools[applicationPoolName];

                if (appPool == null)
                {
                    //throw new Exception(String.Format("Application Pool: {0} does not exist.", applicationPoolName));
                    // Application Pool Doesn't Exist
                    return bFlag;
                }
                else
                {
                    Site site = serverMgr.Sites.FirstOrDefault(k => k.Name == websiteName);

                    if ((site == null) && IsValidPhysicalPath(virtualDirectoryPhysicalPath))
                    {
                        string ipAddress = "*";
                        long highestId = 1;

                        foreach (Site mysite in serverMgr.Sites)
                        {
                            if (mysite.Id > highestId)
                                highestId = mysite.Id;
                        }

                        highestId++;

                        //Creating SiteName and Assigning ID                       
                        site = serverMgr.Sites.CreateElement();
                        site.SetAttributeValue("name", websiteName);
                        site.Id = highestId;

                        site.Bindings.Clear();

                        //Validating HostName and PortNo

                        string bind = ipAddress + ":" + GetUnusedPortNo() + ":" + hostHeader;

                        Binding binding = site.Bindings.CreateElement();
                        binding.Protocol = "http";
                        binding.BindingInformation = bind;
                        site.Bindings.Add(binding);

                        string bind1 = ipAddress + ":" + (int.Parse(GetUnusedPortNo()) + 1) + ":" + hostHeader;

                        Binding binding1 = site.Bindings.CreateElement();
                        binding1.Protocol = "https";
                        binding1.BindingInformation = bind1;
                        site.Bindings.Add(binding1);

                        Application app = site.Applications.CreateElement();
                        app.Path = applicationPath;
                        app.ApplicationPoolName = applicationPoolName;

                        VirtualDirectory vdir = app.VirtualDirectories.CreateElement();
                        vdir.Path = virtualDirectoryPath;
                        vdir.PhysicalPath = virtualDirectoryPhysicalPath;
                        app.VirtualDirectories.Add(vdir);
                        site.Applications.Add(app);

                        serverMgr.Sites.Add(site);
                        serverMgr.CommitChanges();



                        bFlag = true;
                    }
                    else
                    {
                        // Invalid Site Name or Physical Path Specified
                        return bFlag;
                    }

                }
            }
            return bFlag;
        }
        public bool DeleteWebsite(string websiteName)
        {
            bool bFlag = false;
            using (ServerManager serverMgr = new ServerManager())
            {
                Site site = serverMgr.Sites.Where(sitename => sitename.Name == websiteName).FirstOrDefault();
                if (site != null)
                {
                    serverMgr.Sites.Remove(site);
                    serverMgr.CommitChanges();
                    bFlag = true;
                }
            }
            return bFlag;
        }

        bool IsValidPhysicalPath(string PhysicalPath)
        {
            return Directory.Exists(PhysicalPath);
        }


        //string[] ports = { "80", "443" };
        //string[] protocols = { "http", "https" };
        //while (i < 2) {
        //using (ServerManager serverManager = new ServerManager()) {

        //  var smSite = serverManager.Sites.FirstOrDefault(s => s.Name.Contains("MySite"));
        //  if (smSite != null) {
        //    BindingCollection bindingCollection = smSite.Bindings;
        //    Binding binding = smSite.Bindings.CreateElement("binding");
        //    binding["protocol"] = protocols[i];
        //    if (i==1) {
        //     binding["certificateHash"] = "EFA2E9A7D9F032309059CFA0B54D6D281E302007"; // Enter your cert thumbprint value
        //     binding["certificateStoreName"] = "My"; // This is generally the strore name for all certs
        //    }
        //    binding["bindingInformation"] = string.Format("{0}:{1}:{2}", ip, ports[i], hostName);
        //    bindingCollection.Add(binding);
        //    serverManager.CommitChanges();
        //    }
        //    i++;
        //  }
        //}


        public void RemoveSiteWithFolder(string siteName)
        {
            using (ServerManager serverMgr = new ServerManager())
            {

                Site site = serverMgr.Sites.FirstOrDefault(k => k.Name == siteName);
                var data = site.Applications[0].VirtualDirectories[0].PhysicalPath;

            }
        }
    }

}
