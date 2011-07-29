using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using TellagoStudios.Hermes.Client.Tests.Util;

namespace TellagoStudios.Hermes.Client.Tests.IntegrationTests
{
    [SetUpFixture]     
    public class DeployAndStartWebApplication
    {
        const int port = 40403;
        private IISExpress iis;
        private string applicationPath;

        [SetUp]
        public void SetUp()
        {
            applicationPath = CreateATemporalCopy(Path.GetFullPath(@"..\..\..\RestService\"));
            UpdateConnectionString(applicationPath);
            
            iis = IISExpress.Start(applicationPath, port, sysTray: true);
        }

        private void UpdateConnectionString(string applicationPath)
        {
            var newConnectionString = ConfigurationManager.ConnectionStrings["db.connectionString"];
            var webConfigPath = Path.Combine(applicationPath, "web.config");
            var xDocument = XDocument.Load(webConfigPath);
            
            xDocument.Element("configuration")
                .Element("connectionStrings")
                .Elements("add")
                .First(e => e.Attributes().Any(a => a.Name.LocalName == "name" && a.Value == "db.connectionString"))
                .Attributes().FirstOrDefault(a => a.Name == "connectionString")
                .SetValue(newConnectionString);

            xDocument.Element("configuration")
                .Element("appSettings")
                .Elements("add")
                .First(e => e.Attributes().Any(a => a.Name.LocalName == "key" && a.Value == "baseAddress"))
                .Attributes().FirstOrDefault(a => a.Name == "value")
                .SetValue(string.Format("http://localhost:{0}", 40403));

            xDocument.Save(webConfigPath);
        }

        private static string CreateATemporalCopy(string sourceApplicationPath)
        {
            var tempPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName())).FullName;

            var copyProcessInfo = new ProcessStartInfo("xcopy", string.Format("\"{0}\" \"{1}\" /E ", 
                                                        sourceApplicationPath.Substring(0, sourceApplicationPath.Length - 1), 
                                                        tempPath))
                                      {
                                          CreateNoWindow = true,
                                          WindowStyle = ProcessWindowStyle.Hidden
                                      };

            var copyProcess = Process.Start(copyProcessInfo);
            copyProcess.WaitForExit();
            return tempPath;
        }

        [TearDown]
        public void TearDown()
        {
            iis.Stop();
            Directory.Delete(applicationPath, true);
        }

    }
}