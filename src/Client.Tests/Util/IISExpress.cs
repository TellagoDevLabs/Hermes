using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TellagoStudios.Hermes.Client.Tests.Util
{
    public class IISExpress
    {
        internal class NativeMethods
        {
            // Methods
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetTopWindow(IntPtr hWnd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }

        public static void SendStopMessageToProcess(int PID)
        {
            try
            {
                for (var ptr = NativeMethods.GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = NativeMethods.GetWindow(ptr, 2))
                {
                    uint num;
                    NativeMethods.GetWindowThreadProcessId(ptr, out num);
                    if (PID != num) continue;
                    var hWnd = new HandleRef(null, ptr);
                    NativeMethods.PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                    return;
                }
            }
            catch (ArgumentException)
            {
            }
        }

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
                                            RedirectStandardOutput = true,
                                            UseShellExecute = false,
                                            //WindowStyle = ProcessWindowStyle.Minimized,
                                            CreateNoWindow = true
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
            SendStopMessageToProcess(process.Id);
            process.Close();
        }
    }
}
