using System;
using System.Windows.Forms;
using TellagoStudios.Hermes.Facade;

namespace Demo
{
    public partial class SubscriptionForm : Form
    {
        public SubscriptionForm()
        {
            InitializeComponent();

            cbCallback.DisplayMember = "Name";
            cbCallback.ValueMember = "Value";
            cbCallback.Items.Add(new { Name = "None", Value = string.Empty });
            cbCallback.Items.Add(new { Name = CallbackKind.Key.ToString(), Value = CallbackKind.Key });
            cbCallback.Items.Add(new { Name = CallbackKind.Message.ToString(), Value = CallbackKind.Message });
            cbCallback.SelectedIndex = 0;
        }


        internal static SubscriptionPost New(Topic topic)
        {
            return Dialog<SubscriptionForm>.Show(form =>
                                                     {
                                                         form.txtTarget.Text = string.Format("Topic: {0} ({1})", topic.Name, topic.Id);
                                                     },
                                                     form => 
                                                     {
                                                         var post = form.GetPost();
                                                         post.TopicId = topic.Id;
                                                         return post;
                                                     });
        }

        internal static SubscriptionPost New(Group group)
        {
            return Dialog<SubscriptionForm>.Show(form =>
            {
                form.txtTarget.Text = string.Format("Group: {0} ({1})", group.Name, group.Id);
            },
                                                 form => 
                                                 {
                                                     var post = form.GetPost();
                                                     post.GroupId = group.Id;
                                                     return post;
                                                 });
        }

        internal static SubscriptionPut Update(Subscription subscription)
        {

            return Dialog<SubscriptionForm>.Show(form =>
            {
                form.txtID.Text = subscription.Id.ToString();
                form.txtTarget.Text = subscription.Target.HRef.ToString();

                if (!string.IsNullOrWhiteSpace(subscription.Filter))
                {
                    form.txtFilter.Text = subscription.Filter;
                }

                if (subscription.Callback != null)
                {
                    form.cbCallback.SelectedIndex = (subscription.Callback.Kind == CallbackKind.Key) ? 1 : 2;
                    form.txtCallback.Text = subscription.Callback.Url;
                }
            },
            form => 
            {
                var put = new SubscriptionPut 
                { 
                    Id = new Identity(form.txtID.Text)
                };  

                if (!string.IsNullOrEmpty(form.txtFilter.Text))
                {
                    put.Filter = form.txtFilter.Text;
                }

                if (form.cbCallback.SelectedIndex > 0)
                {
                    dynamic item = form.cbCallback.SelectedItem;
                    var kind = (CallbackKind)(item.Value);
                    put.Callback = new Callback
                    {
                        Kind = kind,
                        Url = form.txtCallback.Text
                    };
                }
                return put;
            });
        }

        private SubscriptionPost GetPost()
        {
            var post = new SubscriptionPost();
            if (!string.IsNullOrEmpty(txtFilter.Text))
            {
                post.Filter = txtFilter.Text;
            }

            if (cbCallback.SelectedIndex > 0)
            {
                dynamic item = cbCallback.SelectedItem;
                var kind = (CallbackKind)(item.Value);
                post.Callback = new Callback
                {
                    Kind = kind,
                    Url = txtCallback.Text
                };
            }
            return post;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
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
