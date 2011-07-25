using System;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using TellagoStudios.Hermes.Client;

namespace ExampleDecouplingApplications
{
    public partial class AccountManagement : Form
    {
        public AccountManagement()
        {
            InitializeComponent();
            
            cbAccount.DataSource = Repository.Instance.Accounts;
            cbAccount.DisplayMember = "Name";
            cbAccount.ValueMember = "Id";
            cbAccount.Refresh();
            
            RefreshGrid();
        }

        private void DoMovement(string description, decimal amount)
        {
            var account = (Account)cbAccount.SelectedItem;
            var movement = new Movement 
            {
                AccountId = account.Id, 
                Amount = amount, 
                Description = description
            };

            // Persist Movement
            Repository.Instance.Movements.Add(movement);

            // Publish Movement
            var hermes = new HermesClient();
            hermes.TryPostMessage("Decoupling Applications", "Movements", movement);

            // Refresh View
            RefreshGrid();
        }

        private static void AccountManagmentFormClosed(object sender, FormClosedEventArgs e)
        {
            Repository.Instance.Save();
        }

        private void BtDepositClick(object sender, EventArgs e)
        {
            OnValidEntry((amount, description) => DoMovement(description, amount));
        }

        private void BtWithdrawClick(object sender, EventArgs e)
        {
            OnValidEntry((amount, description) => DoMovement(description, -amount));
        }

        private void OnValidEntry(Action<decimal, string> action)
        {
            decimal amount;
            if (!decimal.TryParse(tbAmount.Text, out amount) && amount > 0
                || amount <= 0)
            {
                SystemSounds.Beep.Play();
                tbAmount.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(tbDescription.Text))
            {
                SystemSounds.Beep.Play();
                tbDescription.Focus();
                return;
            }

            action(amount, tbDescription.Text);
            tbAmount.Text = null;
            tbDescription.Text = null;
            tbDescription.Focus();
        }

        private void RefreshGrid()
        {
            var account = (Account)cbAccount.SelectedItem;
            dgvMovements.DataSource = Repository.Instance.Movements
                .Where(m => m.AccountId == account.Id)
                .Select(m => new { m.Description, m.Amount})
                .ToArray();
            
            dgvMovements.Refresh();
        }

        private void CbAccountSelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void BtClearClick(object sender, EventArgs e)
        {
            Repository.Instance.Movements.Clear();
            RefreshGrid();
        }

        private void BtRefreshClick(object sender, EventArgs e)
        {
            RefreshGrid();
        }
    }
}
