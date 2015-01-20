using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IISApplicationLibrary;
using IISDBLibrary;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.IO;
using System.Data.SqlClient;

namespace IISApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        IISLibrary web = new IISLibrary();
        private void button1_Click(object sender, EventArgs e)
        {

            //web.DeleteWebsite("Test1");

            var data = web.CreateWebsite("Test1", @"C:\inetpub\wwwroot\Test");
            //var data2 = web.CreateWebsite("Test2", @"C:\inetpub\wwwroot\Test", false);
            //IISApplicationLibrary.IISLibrary.CreateWebsite("SampleWebsite","1234");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DBLibrary db = new DBLibrary();
            //db.CreateDb("net", DBCompatibilityType.SqlServer2012);
            if (db.IsDatabaseExist("netCopy", "COMPAQ", "sa", "Infotech@1"))
            {
                db.DropDb("netCopy", false, "COMPAQ", "sa", "Infotech@1");
            }

            db.CreateDb("COMPAQ", "sa", "Infotech@1", "net", "COMPAQ", "sa", "Infotech@1");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //TcpListener l = new TcpListener(IPAddress.Loopback, 0);
            //l.Start();
            //int port = ((IPEndPoint)l.LocalEndpoint).Port;
            //l.Stop();
            web.RemoveSiteWithFolder("test");

            MoveData(@"C:\Users\Sys\Desktop\A", @"C:\Users\Sys\Desktop\B");

        }

        public void MoveData(string SourcePath, string DestinationPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(SourcePath, DestinationPath), true);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlConnection conn = new SqlConnection("server=COMPAQ;uid=route;pwd=route;database=ActiveDirectory;");
            DataSet resultSet = new DataSet();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Clear();
            command.CommandText = "FetchesClients";

            dataAdapter.SelectCommand = command;
            dataAdapter.Fill(resultSet);
            conn.Close();
        }

    }
}
