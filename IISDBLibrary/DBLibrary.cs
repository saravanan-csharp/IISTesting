using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using System.Diagnostics;



namespace IISDBLibrary
{
    public enum DBCompatibilityType
    {
        SqlServer2014,
        SqlServer2012,
        SqlServer2008R2
    }
    public class DBLibrary
    {
        Server sourceServer;
        Server destinationServer;
        public DBLibrary()
        {
            sourceServer = new Server();
            destinationServer = new Server();
        }
        public void CreateDb(string sourceServerName, string sourceUserName, string sourcePassword, string DBName, string destinationServerName, string destinationUserName, string destinationPassword)
        {

            sourceServer.ConnectionContext.ServerInstance = sourceServerName;
            sourceServer.ConnectionContext.ConnectAsUserName = sourceUserName;
            sourceServer.ConnectionContext.ConnectAsUserPassword = sourcePassword;
            sourceServer.ConnectionContext.Connect();

            destinationServer.ConnectionContext.ServerInstance = destinationServerName;
            destinationServer.ConnectionContext.ConnectAsUserName = destinationUserName;
            destinationServer.ConnectionContext.ConnectAsUserPassword = destinationPassword;
            destinationServer.ConnectionContext.Connect();

            if (!IsValidDestinationDB(destinationServerName, destinationUserName, destinationPassword, DBName + "Copy"))
            {
                if (sourceServer.ConnectionContext.IsOpen && destinationServer.ConnectionContext.IsOpen)
                {
                    Database sourceDB = sourceServer.Databases[DBName];

                    Database destinationDB = new Database(destinationServer, DBName + "Copy");
                    destinationDB.CompatibilityLevel = sourceDB.CompatibilityLevel;
                    destinationDB.Create();

                    TransferData(sourceDB, destinationServerName, destinationDB.Name);

                    CreateUser(destinationServer, destinationDB);
                }
            }

            sourceServer.ConnectionContext.Disconnect();

            destinationServer.ConnectionContext.Disconnect();
        }

        public void DropDb(string DBName, bool IsBackupRequired, string destinationServerName, string destinationUserName, string destinationPassword)
        {
            Server destinationServer = new Server();
            destinationServer.ConnectionContext.ServerInstance = destinationServerName;
            destinationServer.ConnectionContext.ConnectAsUserName = destinationUserName;
            destinationServer.ConnectionContext.ConnectAsUserPassword = destinationPassword;
            destinationServer.ConnectionContext.Connect();

            if (destinationServer.ConnectionContext.IsOpen)
            {
                Database database = destinationServer.Databases[DBName];
                if (IsBackupRequired)
                {
                    database.Drop();
                }
                else
                {                    
                    database.DropBackupHistory();
                    database.Drop();
                }
            }
            
            destinationServer.ConnectionContext.Disconnect();
        }

        public bool IsDatabaseExist(string DBName, string serverName, string userName, string password)
        {
            bool bFlag = false;
            Server destinationServer = new Server();
            destinationServer.ConnectionContext.ServerInstance = serverName;
            destinationServer.ConnectionContext.ConnectAsUserName = userName;
            destinationServer.ConnectionContext.ConnectAsUserPassword = password;
            destinationServer.ConnectionContext.Connect();

            if (destinationServer.ConnectionContext.IsOpen)
            {
                Database database = destinationServer.Databases[DBName];
                if (database != null)
                {
                    bFlag = true;
                }
            }

            destinationServer.ConnectionContext.Disconnect();

            return bFlag;
        }
        void CreateUser(Server destinationServer, Database destinationDB)
        {
            Login login = new Login(destinationServer, "Login1");
            login.LoginType = LoginType.SqlLogin;
            login.Create("password@1");

            User user1 = new User(destinationDB, "Login1");
            user1.Login = "Login1";
            user1.Create();
            user1.AddToRole("db_owner");

            
            
            DatabasePermissionSet dbPermSet = new DatabasePermissionSet(DatabasePermission.TakeOwnership);
            dbPermSet.Add(DatabasePermission.TakeOwnership);

            // Granting Database Permission Sets to Roles
            destinationDB.Grant(dbPermSet, "Login1");
            
        }
        void TransferData(Database sourceDB, string ServerName, string DBName)
        {
            Transfer transfer = new Transfer(sourceDB);
            transfer.CopyAllDatabaseTriggers = true;
            transfer.CopyAllDefaults = true;
            transfer.CopyAllRules = true;
            transfer.CopyAllSequences = true;
            transfer.CopyAllStoredProcedures = true;
            transfer.CopyAllSynonyms = true;
            transfer.CopyAllTables = true;
            transfer.CopyAllUserDefinedAggregates = true;
            transfer.CopyAllUserDefinedDataTypes = true;
            transfer.CopyAllUserDefinedFunctions = true;
            transfer.CopyAllUserDefinedTableTypes = true;
            transfer.CopyAllUserDefinedTypes = true;
            transfer.CopyAllViews = true;

            transfer.CopyData = true;

            transfer.Options.WithDependencies = true;
            transfer.Options.ContinueScriptingOnError = true;
            transfer.DestinationDatabase = DBName;
            transfer.DestinationServer = ServerName;

            transfer.DestinationLoginSecure = true;

        
            transfer.TransferData();

        }

        bool IsValidDestinationDB(string destinationServerName, string destinationUserName, string destinationPassword, string DatabaseName)
        {
            bool bFlag = false;
            Server server = new Server();

            server.ConnectionContext.ServerInstance = destinationServerName;
            server.ConnectionContext.ConnectAsUserName = destinationUserName;
            server.ConnectionContext.ConnectAsUserPassword = destinationPassword;
            server.ConnectionContext.Connect();

            bFlag = server.Databases.Contains(DatabaseName);

            server.ConnectionContext.Disconnect();

            return bFlag;

        }
    }
}
