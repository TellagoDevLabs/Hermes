using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TellagoStudios.Hermes.Client;
using TellagoStudios.Hermes.RestService.Facade;
using Microsoft.ApplicationServer.Http.Activation;
using Microsoft.ApplicationServer.Http.Description;

namespace Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            HermesClient.GetUrl = () => txtUrl.Text;
            
            RefreshGroups();
            RefreshSubscriptions();
        }

        public string Url { get { return txtUrl.Text; } }

        private void btGroupRefresh_Click(object sender, EventArgs e)
        {
            RefreshGroups();
        }

        private void btAddGroup_Click(object sender, EventArgs e)
        {
            var groupPost = GroupForm.New();
            if (groupPost != null)
            {
                HermesClient.NewAdmin().CreateGroup(groupPost);
                RefreshGroups();
            }
        }

        private void RefreshGroups()
        {
            tvGroups.Nodes.Clear();

            var groups = HermesClient.NewAdmin().GetGroups();

            if (groups == null)
                return;

            bool updated;
            do
            {
                updated = false;
                foreach (var group in groups)
                {
                    var node = tvGroups.Nodes.Find(group.Id.ToString());
                    
                    if (node != null) 
                        continue;

                    if (group.Parent == null)
                    {
                        var newNode = tvGroups.Nodes.Add(group.Id.ToString(), group.Name);
                        newNode.ToolTipText = group.Id + " " + group.Description;
                        newNode.Tag = group;
                        updated = true;
                    }
                    else
                    {
                        var parent = tvGroups.Nodes.Find(group.Parent.GetId());
                        if (parent != null)
                        {
                            var newNode = parent.Nodes.Add(group.Id.ToString(), group.Name);
                            newNode.ToolTipText = group.Id + " " + group.Description;
                            newNode.Tag = group;
                            updated = true;
                        }
                    }
                }
            } while (updated);
         
            tvGroups.Refresh();
            tvGroups.ExpandAll();
        }

        private void btUpdateGroup_Click(object sender, EventArgs e)
        {
            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;
            
            var groupPut = GroupForm.Update(group);
            if (groupPut != null)
            {
                HermesClient.NewAdmin().UpdateGroup(groupPut);
                RefreshGroups();
            }
        }

        private void btDeleteGroup_Click(object sender, EventArgs e)
        {
            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;
            var result = MessageBox.Show(@"Do you want to remove the group '" + group.Name + @"'", 
                @"Remove group",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                HermesClient.NewAdmin().DeleteGroup(group.Id);
                RefreshGroups();
            }
        }

        private void tvGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshTopics();
        }

        private void RefreshTopics()
        {
            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;
            var topics = HermesClient.NewAdmin().GetTopicsByGroup(group.Id);


            dgvTopics.DataSource = (topics ?? new Topic[0])
                    .Select(t => new {Data = t, ID = t.Id, Name = t.Name, Description = t.Description})
                    .ToArray();
            dgvTopics.Refresh();
            dgvTopics.Columns[0].Visible = false;
        }

        private void btTopicRefresh_Click(object sender, EventArgs e)
        {
            RefreshTopics();
        }

        private void btTopicAdd_Click(object sender, EventArgs e)
        {
            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;
            var topicPost = TopicForm.New(group);
            if (topicPost != null)
            {
                HermesClient.NewAdmin().CreateTopic(topicPost);
                RefreshTopics();
            }
        }

        private void btTopicUpdate_Click(object sender, EventArgs e)
        {
            if (dgvTopics.SelectedRows.Count == 0) return;
            var topic = (Topic)dgvTopics.SelectedRows[0].Cells[0].Value;

            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;

            var topicPut = TopicForm.Update(group, topic);
            if (topicPut != null)
            {
                HermesClient.NewAdmin().UpdateTopic(topicPut);
                RefreshTopics();
            }
        }

        private void btTopicDelete_Click(object sender, EventArgs e)
        {
            if (dgvTopics.SelectedRows.Count == 0) return;
            var topic = (Topic)dgvTopics.SelectedRows[0].Cells[0].Value;

            var result = MessageBox.Show(@"Do you want to remove the topic '" + topic.Name + @"'",
                @"Remove topic",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                HermesClient.NewAdmin().DeleteTopic(topic.Id);
                RefreshTopics();
            }
        }

        private void RefreshSubscriptions()
        {
            Subscription[] subscriptions = HermesClient.NewAdmin().GetSubscriptions();
            RefreshSubscriptions(subscriptions);
        }

        private void RefreshSubscriptions(IEnumerable<Subscription> subscriptions)
        {
            if (subscriptions == null)
                return;

            dgvSubscriptions.DataSource = subscriptions
                .Select(s => new
                {
                    Data = s,
                    ID = s.Id,
                    Target = s.Target.href,
                    Filter = s.Filter,
                    Callback = s.Callback == null ? null : s.Callback.Kind.ToString() + " " + s.Callback.Url
                })
                .ToArray();
            dgvSubscriptions.Refresh();
            dgvSubscriptions.Columns[0].Visible = false;
        }

        private void btAddSubscriptionToGroup_Click(object sender, EventArgs e)
        {
            var node = tvGroups.SelectedNode;
            if (node == null) return;

            var group = node.Tag as Group;

            var post = SubscriptionForm.New(group);
            if (post != null)
            {
                HermesClient.NewAdmin().CreateSubscription(post);
                RefreshSubscriptions();
            }
        }

        private void btAddSubscriptionToTopic_Click(object sender, EventArgs e)
        {
            if (dgvTopics.SelectedRows.Count == 0) return;
            var topic = (Topic)dgvTopics.SelectedRows[0].Cells[0].Value;

            var post = SubscriptionForm.New(topic);
            if (post != null)
            {
                HermesClient.NewAdmin().CreateSubscription(post);
                RefreshSubscriptions();
            }
        }

        private void btRefreshSubs_Click(object sender, EventArgs e)
        {
            RefreshSubscriptions();
        }

        private void btDeleteSubs_Click(object sender, EventArgs e)
        {
            if (dgvSubscriptions.SelectedRows.Count == 0) return;
            var subscription = (Subscription)dgvSubscriptions.SelectedRows[0].Cells[0].Value;

            var result = MessageBox.Show(@"Do you want to remove the subscription '" + subscription.Id + @"'",
                @"Remove subscription",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                HermesClient.NewAdmin().DeleteSubscription(subscription.Id);
                RefreshSubscriptions();
            }
        }

        private void btUpdateSubs_Click(object sender, EventArgs e)
        {
            if (dgvSubscriptions.SelectedRows.Count == 0) return;
            var subscription = (Subscription)dgvSubscriptions.SelectedRows[0].Cells[0].Value;

            var put = SubscriptionForm.Update(subscription);
            if (put != null)
            {
                HermesClient.NewAdmin().UpdateSubscription(put);
                RefreshSubscriptions();
            }
        }

        private void btRefreshMessages_Click(object sender, EventArgs e)
        {
            RefreshMessages();
        }

        private void RefreshMessages()
        {
            if (dgvSubscriptions.SelectedRows.Count == 0) return;
            var subscription = (Subscription)dgvSubscriptions.SelectedRows[0].Cells[0].Value;

            var messages = HermesClient.NewAdmin().GetMessagesLink(subscription.Id);

            lbMessages.DataSource = messages 
                .Select(m=>new {Text = m.ToXmlString(), Value=m})
                .ToArray();
            lbMessages.DisplayMember = "Text";
            lbMessages.ValueMember = "Value";
            lbMessages.Refresh();
        }

        private void RefreshStartButton()
        {
            var result = false;
            if (dgvSubscriptions.SelectedRows.Count != 0)
            {
                var subscription = (Subscription) dgvSubscriptions.SelectedRows[0].Cells[0].Value;
                result = subscription.Callback != null && 
                    string.Compare(new Uri(subscription.Callback.Url).Host, "localhost", true)==0 &&
                    !IsHostAlreadyRunning(new Uri(subscription.Callback.Url));
            }

            btStartListener.Enabled = result;
        }

        private void dgvSubscriptions_SelectionChanged(object sender, EventArgs e)
        {
            RefreshMessages();
            RefreshStartButton();
        }

        private void btTopicPublish_Click(object sender, EventArgs e)
        {
            if (dgvTopics.SelectedRows.Count == 0) return;

            var topic = (Topic)dgvTopics.SelectedRows[0].Cells[0].Value;

            MessagesForm.PublishOnTopic(topic.Id);
        }

        private void lbMessages_DoubleClick(object sender, EventArgs e)
        {
            GetMessage();
        }

        private void btMessageGet_Click(object sender, EventArgs e)
        {
            GetMessage();
        }

        private void GetMessage()
        {
            if (lbMessages.SelectedIndex == -1) return;

            var link = (Link) lbMessages.SelectedValue;

            var response = HermesClient.NewAdmin().GetMessage(link.href); 

            RequestResult.ShowResponse(response);
        }

        private void showSubscriptionsForThisGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var id = tvGroups.SelectedNode.Name;
            var subscriptions = HermesClient.NewAdmin().GetSubscriptionsByGroup(new Identity(id));
            RefreshSubscriptions(subscriptions);
        }

        private void showSubscriptionsForTopicMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvTopics.SelectedRows.Count == 0) return;
            var selectedRow = dgvTopics.SelectedRows[0];

            dynamic data = selectedRow.DataBoundItem;
            var id = data.ID;

            Subscription[] subscriptions = HermesClient.NewAdmin().GetSubscriptionsByTopic(id);
            RefreshSubscriptions(subscriptions);
        }

        private static readonly IList<Form> openedForms = new List<Form>();

        public static void AddForm(Form form)
        {
            if (form==null) return;
            openedForms.Add(form);
            form.FormClosed += form_FormClosed;
        }

        static void  form_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = (Form) sender;
            if (openedForms.Contains(form))
            {
                openedForms.Remove(form);
            }
        }

        private void btStartListener_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (dgvSubscriptions.SelectedRows.Count == 0) return;

                var subscription = (Subscription)dgvSubscriptions.SelectedRows[0].Cells[0].Value;
                if (subscription.Callback == null) return;

                var callbackUri = new Uri(subscription.Callback.Url);
                if (IsHostAlreadyRunning(callbackUri)) return;

                // Hosts the REST service
                var newUrl = callbackUri.GetLeftPart(UriPartial.Authority);

                var config = HttpHostConfiguration.Create();
                var host = new HttpConfigurableServiceHost(typeof(CallbackListenerService), config, new Uri(newUrl));
                host.Open();
                lbListeners.Items.Add(new HostItem
                                          {
                                              Url = newUrl,
                                              Host = host
                                          });
                RefreshStartButton();
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private bool IsHostAlreadyRunning(Uri callbackUri)
        {
            var uri = callbackUri.GetLeftPart(UriPartial.Authority);
            return lbListeners.Items
                .Cast<HostItem>()
                .Any(hi => hi.Url == uri);
        }

        private void btStopListener_Click(object sender, EventArgs e)
        {
            var items = lbListeners.SelectedItems
                .Cast<HostItem>()
                .ToArray();

            items.ForEach(i =>
                              {
                                  lbListeners.Items.Remove(i);
                                  i.Host.Close();
                              });
        }

        private void txtUrl_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class HostItem
    {
        public string Url;
        public HttpConfigurableServiceHost Host;
        public override string ToString()
        {
            return Url;
        }
    }
}