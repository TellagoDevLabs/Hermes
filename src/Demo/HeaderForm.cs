using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TellagoStudios.Hermes.Client;
using TellagoStudios.Hermes.RestService.Facade;

namespace Demo
{
    public partial class HeaderForm : Form
    {
        public HeaderForm()
        {
            InitializeComponent();
        }

        internal static Header New()
        {
            return Dialog<HeaderForm>.Show(f => new Header(f.txtName.Text, f.txtValue.Text));
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Focus();
                return;
            }

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
