using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using TellagoStudios.Hermes.Client;
using TellagoStudios.Hermes.Facade;

namespace Demo
{
    public partial class MessagesForm : Form
    {
        public MessagesForm()
        {
            InitializeComponent();

            cbContentType.Items.Add("text/text");
            cbContentType.Items.Add("application/xml");
            cbContentType.Items.Add("application/json");
            cbContentType.SelectedIndex = 0;

            cbPP.Items.Add("Create one header by property.");
            cbPP.Items.Add("Create only one header for all properties");
            cbPP.SelectedIndex = 0;

            lbHeaders.Items.Add(new Header ( "Accept", "application/xml" ));
        }

        internal static void PublishOnTopic(Identity topicId)
        {
            var message = Dialog<MessagesForm>.Show(
                f =>
                {
                    var headers = f.lbHeaders.Items
                        .Cast<Header>()
                        .ToList();

                    headers.Add(new Header { Name = HttpRequestHeader.ContentType.ToString(), Value = f.cbContentType.Text });

                    var promotedProperties = f.lbPP.Items
                        .Cast<Header>();

                    Stream stream = null;
                    if (f.txtContent.Text!=null)
                    {
                        stream = new MemoryStream();
                        var writer = new StreamWriter(stream);
                        writer.Write(f.txtContent.Text);
                        writer.Flush();
                        stream.Seek(0, SeekOrigin.Begin);
                    }

                    var msg = new TellagoStudios.Hermes.Client.Message
                    {
                        TopicId = topicId,
                        Payload = stream,
                        Headers = headers,
                        PromotedProperties = promotedProperties
                    };

                    return msg;
                });

            if (message != null)
            {
                HermesClient.NewAdmin().PostMessage(message);
            }
        }

        private void btAddHeader_Click(object sender, EventArgs e)
        {
            var header = HttpHeaderForm.New();

            if (header != null)
                lbHeaders.Items.Add(header);
        }

        private void btRemoveHeader_Click(object sender, EventArgs e)
        {
            if (lbHeaders.SelectedIndex >= 0)
                lbHeaders.Items.RemoveAt(lbHeaders.SelectedIndex);
        }

        private void btAddPP_Click(object sender, EventArgs e)
        {
            var header = HeaderForm.New();
            if (header != null)
                lbPP.Items.Add(header);
        }

        private void btRemovePP_Click(object sender, EventArgs e)
        {
            if (lbPP.SelectedIndex >= 0)
                lbPP.Items.RemoveAt(lbPP.SelectedIndex);
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContent.Text))
            {
                txtContent.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
