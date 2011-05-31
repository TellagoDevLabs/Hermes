using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Demo
{
    public partial class MessageRecievedForm : Form
    {
        public MessageRecievedForm()
        {
            InitializeComponent();
        }


        public static void ShowRequest(HttpWebRequest request)
        {
           if (request==null) throw new ArgumentNullException("request");

            var sb = new StringBuilder();
            foreach (var headerKey in request.Headers.AllKeys)
            {
                sb.AppendFormat("{0} = {1}", headerKey, request.Headers[headerKey]);
                sb.AppendLine();
            }
            sb.AppendLine();

            var stream = request.GetRequestStream();
            var reader = new StreamReader(stream);
            sb.Append(reader.ReadToEnd());

            var form = new MessageRecievedForm
                           {
                               Text = request.Method + @" " + request.RequestUri,
                               txt =
                                   {
                                       Text = sb.ToString(),
                                       SelectionStart = 0,
                                       SelectionLength = 0
                                   }
                           };
            form.Show();
            MainForm.AddForm(form);
        }
    }
}
