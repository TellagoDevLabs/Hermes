using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class RequestResult : Form
    {
        public RequestResult()
        {
            InitializeComponent();
        }

        public static void ShowRequest(HttpWebRequest request, string caption = "Result")
        {
            ShowRequestResponse(request, null, caption);
        }

        public static void ShowResponse(HttpWebResponse response, string caption = "Result")
        {
            ShowRequestResponse(null, response, caption);
        }

        public static void ShowRequestResponse(HttpWebRequest request, HttpWebResponse response, string caption = "Result")
        {
            var sb = new StringBuilder();

            if (request != null)
            {
                if (response != null)
                {
                    sb.AppendLine("----------------------------- REQUEST ----------------------------------");
                }
                sb.AppendFormat("{0} {1}", request.Method, request.RequestUri);
                sb.AppendLine();
                sb.AppendLine();
                foreach (var headerKey in request.Headers.AllKeys)
                {
                    sb.AppendFormat("{0} = {1}", headerKey, request.Headers[headerKey]);
                    sb.AppendLine();
                }
                sb.AppendLine();

                var stream = request.GetRequestStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    sb.Append(reader.ReadToEnd());
                }
            }

            if (response != null)
            {
                if (request != null)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                    sb.AppendLine("----------------------------- RESPONSE ----------------------------------");
                    sb.AppendLine();
                }
                sb.AppendFormat("Status = {0} ({1})", response.StatusCode, (int)response.StatusCode);
                sb.AppendLine();
                sb.AppendLine();

                foreach (var headerKey in response.Headers.AllKeys)
                {
                    sb.AppendFormat("{0} = {1}", headerKey, response.Headers[headerKey]);
                    sb.AppendLine();
                }
                sb.AppendLine();

                var stream = response.GetResponseStream();
                if (stream != null)
                {
                    var reader = new StreamReader(stream);
                    sb.Append(reader.ReadToEnd());
                }

                var form = new RequestResult();
                form.txt.Text = sb.ToString();
                form.txt.SelectionStart = 0;
                form.txt.SelectionLength = 0;
                form.Show();
                MainForm.AddForm(form);
            }
        }
    }
}