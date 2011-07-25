using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ExampleDecouplingApplications
{
    class DemoApplicationContext : ApplicationContext
    {
        readonly List<Form> forms = new List<Form>();

        public DemoApplicationContext()
        {
            var accountManagement =Launch<AccountManagement>();
            var alerts = Launch<Alerts>();

            // Arrange windows
            alerts.Location = new Point(accountManagement.Left + accountManagement.Width + 20, accountManagement.Top);

        }

        T Launch<T>() where T : Form, new()
        {

            var form = new T();
            forms.Add(form);
            form.FormClosed += FormClosed;
            form.Show();
            return form;
        }

        void FormClosed(object sender, FormClosedEventArgs e)
        {
            forms.Remove((Form)sender);
            if (forms.Count == 0) ExitThread();
        }  
    }
}
