using System;
using System.Windows.Forms;

namespace Demo
{
    static class Dialog<TForm>
        where TForm : Form, new()
    {
        public static TEntity Show<TEntity>(Func<TForm, TEntity> action)
        {
            return Show(null, action);
        }

        public static TEntity Show<TEntity>(Action<TForm> startup, Func<TForm, TEntity> action)
        {
            TEntity t = default(TEntity);
            using (var form = new TForm())
            {
                if (startup != null)
                {
                    startup(form);
                }
                if (form.ShowDialog() == DialogResult.OK)
                {
                    t = action(form);
                }
                form.Close();
            }
            return t;
        }
    }
}
