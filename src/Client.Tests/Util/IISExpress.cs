using System;
using System.Diagnostics;
using System.Text;

namespace TellagoStudios.Hermes.Client.Tests.Util
{
    public class IISExpress
    {
       
        private string IIS_EXPRESS = string.Format(
                    @"{0}\IIS Express\iisexpress.exe", 
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

        const string PATH = "path";
        const string PORT = "port";
        const string SYSTRAY = "systray";
        const string CLR = "clr";

        Process process;

        IISExpress(string path, int port = 8080, string clr = "v4.0", bool sysTray = true)
        {
            Path = path;
            Port = port;
            AppPool = sysTray;

            var arguments = new StringBuilder();
            if (!string.IsNullOrEmpty(Path))
                arguments.AppendFormat("/{0}:{1} ", PATH, path);

            if (Port != 8080)
                arguments.AppendFormat("/{0}:{1} ", PORT, Port);

            if (!sysTray)
                arguments.AppendFormat("/{0}:{1} ", SYSTRAY, false);

            if (clr != "v4.0")
                arguments.AppendFormat("/{0}:{1} ", CLR, clr);

            process = Process.Start(new ProcessStartInfo()
                                        {
                                            FileName = IIS_EXPRESS,
                                            Arguments = arguments.ToString(),
                                        });
        }

        public string Path { get; protected set; }
        public int Port { get; protected set; }
        public bool AppPool { get; protected set; }

        public static IISExpress Start(string path, int port = 8080, string clr = "v4.0", bool sysTray = true)
        {
            return new IISExpress(path, port , clr ,sysTray );
        }

        public void Stop()  
        {
            process.Kill();
            process.WaitForExit();
            //process.Close();
            
        }
    }
}
