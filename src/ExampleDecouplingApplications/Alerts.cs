using System;
using System.IO;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Forms;
using TellagoStudios.Hermes.Client;

namespace ExampleDecouplingApplications
{
    public partial class Alerts : Form
    {
        private const string FileName = "Alerts data.txt";
        
        IDisposable subscription;
        Uri lastMessage;

        public Alerts()
        {
            InitializeComponent();

            // Load last message retrived
            LoadData();

            subscription = new HermesClient()
                .TryCreateGroup("Decoupling Applications")  // Get group
                .TryCreateTopic("Movements")                // Get Topic
                .PollMessages<Movement>(                    // Poll Topic's messages 
                    TimeSpan.FromSeconds(1),                //  - every second
                    lastMessage,                            //  - from the 'last' message
                    new ControlScheduler(this))             //  - using the scheduler for WinForm
                .Where(m => m.Data.Amount<=-1000)           // Apply filer
                .Subscribe(ProcessMessage);                 // Process message
        }
        
        private void ProcessMessage(Message<Movement> message)
        {
            // store last message received
            lastMessage = message.Url;

            // TODO: send mail
            lbAlerts.Items.Add(string.Format("Withdraw notification mail was sent to client. Id={0}. Amount={1}",
                                                 message.Data.AccountId, message.Data.Amount));
        }

        #region Private members

        void AlertsFormClosing(object sender, FormClosingEventArgs e)
        {
            // Release subscription
            if (subscription != null)
            {
                subscription.Dispose();
                subscription = null;
            }

            // Save last message received
            SaveData();
        }

        private void LoadData()
        { 
            lastMessage = null;
            try
            {
                if (File.Exists(FileName))
                {
                    using (var file = File.OpenText(FileName))
                    {
                        lastMessage = new Uri(file.ReadLine());
                    }
                }
            }
            catch(Exception)
            {
            }
        }

        private void SaveData()
        {
            try
            {
                if (lastMessage != null)
                {
                    using (var file = File.CreateText(FileName))
                        file.WriteLine(lastMessage.ToString());
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}
