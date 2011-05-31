using System;
using System.Windows.Forms;
using TellagoStudios.Hermes.RestService.Facade;

namespace Demo
{
    public partial class TopicForm : Form
    {
        public TopicForm()
        {
            InitializeComponent();
        }

        internal static TopicPost New(Group group)
        {
            return Dialog<TopicForm>.Show(form =>
            {
                form.txtGroup.Text = group.Name;
            },
                form => new TopicPost
            {
                Description = form.txtDescription.Text,
                Name = form.txtName.Text,
                GroupId = group.Id
            });
        }

        internal static TopicPut Update(Group group, Topic topic)
        {

            return Dialog<TopicForm>.Show(form =>
            {
                form.txtID.Text = topic.Id.ToString();
                form.txtDescription.Text = topic.Description;
                form.txtName.Text = topic.Name;
                form.txtGroup.Text = group.Name;
            },
            form => new TopicPut
            {
                Id = new Identity(form.txtID.Text),
                Description = form.txtDescription.Text,
                Name = form.txtName.Text,
                GroupId = group.Id
            });
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
