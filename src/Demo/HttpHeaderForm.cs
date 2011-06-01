using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.Client;

namespace Demo
{
    public partial class HttpHeaderForm : Form
    {
        public HttpHeaderForm()
        {
            InitializeComponent();

            cbName.Sorted = true;
            cbName.Items.AddRange(Enum.GetNames(typeof(HttpRequestHeader)));
            cbName.SelectedIndex = 0;
        }

        internal static Header New()
        {
            return Dialog<HttpHeaderForm>.Show(f => new Header( f.cbName.Text, f.txtValue.Text ));
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtValue.Text))
            {
                txtValue.Focus();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
