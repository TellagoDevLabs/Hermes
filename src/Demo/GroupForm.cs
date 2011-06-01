using System;
using System.Linq;
using System.Windows.Forms;
using TellagoStudios.Hermes.Facade;
using TellagoStudios.Hermes.Client;

namespace Demo
{
    public partial class GroupForm : Form
    {
        public GroupForm()
        {
            InitializeComponent();
            PopulateParents();
        }

        internal static GroupPost New()
        {
            return Dialog<GroupForm>.Show(form => new GroupPost
                                        {
                                            Description = form.txtDescription.Text,
                                            Name = form.txtName.Text,
                                            ParentId = GetParentId(form.cbParent.SelectedValue)
                                        });
        }

        internal static GroupPut Update(Group group)
        {
           
            return Dialog<GroupForm>.Show(form =>
                              {   
                                  form.txtID.Text = group.Id.ToString();
                                  form.txtDescription.Text = group.Description;
                                  form.txtName.Text = group.Name;
                                  form.cbParent.Select(group.Parent.GetId());
                              },
                            form => new GroupPut
                                {
                                    Id = new Identity(form.txtID.Text),
                                    Description = form.txtDescription.Text,
                                    Name = form.txtName.Text,
                                    ParentId = GetParentId(form.cbParent.SelectedValue)
                                });
        }

        private static Identity? GetParentId(object p)
        {
            if (p == null) return null;
            var g = (Identity)p;
            if (g == Identity.Empty) return null;
            return g;
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
        
       
        private void PopulateParents()
        {
            var groups = HermesClient.NewAdmin().GetGroups().ToList();

            groups.Insert(0, new Group { Id = Identity.Empty, Name = " ------ " });

            cbParent.DataSource = groups;
            cbParent.DisplayMember = "Name";
            cbParent.ValueMember = "Id";
            cbParent.Refresh();
        }

    }
}
