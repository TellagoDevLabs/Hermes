using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using MongoDB.Bson.Serialization;
using NUnit.Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Repository
{
    [TestFixture]
    public class  MongoDbBaseFixture
    {
        private Process mongoProcess;
        private string mongoDir;

        protected string port = "55555";
        protected string connectionString;
        protected MongoServer mongoServer;
        protected MongoDatabase mongoDb;

        
        [TestFixtureSetUp]
        public virtual void FixtureSetUp()
        {
            connectionString = "mongodb://localhost:" + this.port + "/" + MongoDbConstants.DBName;

            #region Launchs MongoDB service in a local process.

            var id = WindowsIdentity.GetCurrent();
            Assert.IsNotNull(id);
            var principal = new WindowsPrincipal(id);
            Assert.IsTrue(principal.IsInRole(WindowsBuiltInRole.Administrator), "Process must run as administrator");

            mongoDir = Path.Combine(Environment.CurrentDirectory, "MongoDb");
            var fileName = "mongod.exe";
            var fileNamePath = Path.Combine(mongoDir, fileName);

            // Kill previous process
            var previous = Process.GetProcessesByName("mongod");

            var current = previous
                .Where(p => p.MainWindowTitle == fileNamePath)
                .FirstOrDefault();

            if (current != null)
            {
                current.Kill();
                Thread.Sleep(1000);
            }

            // Create local folder
            if (Directory.Exists(mongoDir))
            {
                Directory.Delete(mongoDir, true);
            }
            Directory.CreateDirectory(mongoDir);

            // deploy mongod.exe file
            var resourceFileName = Utils.Is64BitOperatingSystem() ? "mongod_64.exe" : "mongod_32.exe";
            using (var resource = typeof(MongoDbBaseFixture).Assembly.GetManifestResourceStream("DataAccess.Tests." + resourceFileName))
            {
                using (var file = File.Create(fileNamePath))
                {
                    const int size = 16384;
                    var buffer = new byte[size];
                    var bytesRead = resource.Read(buffer, 0, size);
                    while (bytesRead > 0)
                    {
                        file.Write(buffer, 0, bytesRead);
                        bytesRead = resource.Read(buffer, 0, size);
                    }
                }
            }

            // Creates mongo process
            var info = new ProcessStartInfo
                           {
                               Arguments = "--dbpath \"" + mongoDir + "\" --port " + port,
                               CreateNoWindow = false,
                               FileName = fileNamePath,
                               WorkingDirectory = mongoDir,
                               UseShellExecute = true,
                               Verb="runas"
                           };
            mongoProcess = Process.Start(info);
            
            #endregion

            BsonSerializer.RegisterSerializer(typeof(Identity), new IdentitySerializer());
            BsonSerializer.RegisterIdGenerator(typeof(Identity?), new IdentityGenerator());

            // Connects to mongo
            mongoServer = MongoServer.Create(connectionString);
            mongoDb = mongoServer.GetDatabase(MongoDbConstants.DBName);

            // Validates that mongo is working
            var col = mongoDb.GetCollection("ping");
            col.Save(new BsonDocument("ping", new BsonInt32(1)));
        }


        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            // Disconnects from Mongo
            if (mongoServer!=null)
            {
                mongoServer.Disconnect();
                mongoServer = null;
            }

            // Close mongo 
            if (mongoProcess!=null)
            {
                mongoProcess.Kill();
                mongoProcess = null;
            }

            // Remove local folder
            Thread.Sleep(1000);
            Directory.Delete(mongoDir, true);
        }
    }
}
