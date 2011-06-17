using System.Linq;
using TellagoStudios.Hermes.Facade;

namespace Demo
{
    public static class Extensions
    {
        static public Identity GetId (this Link link)
        {
            if (link == null || link.HRef == null) return Identity.Empty;

            var id = link.HRef.Segments.Last();
            return new Identity(id);
        }

        static public string ToXmlString (this Link link)
        {
            return string.Format("<link rel=\"{0}\" href=\"{1}\" />", link.Rel, link.HRef);
        }
    }
}

namespace System.Collections.Generic
{
    public static class Extensions
    {
        static public void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;

            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}

namespace  System.Windows.Forms
{
    public static class Extensions
    {
        static public TreeNode Find(this TreeNodeCollection collection, string key)
        {
            if (collection == null || collection.Count == 0) return null;

            if (collection.ContainsKey(key)) return collection[key];

            foreach(TreeNode node in collection)
            {
                var found = node.Nodes.Find(key);
                if (found != null) return found;
            }
            return null;
        }

        static public void Select(this ComboBox control, object value)
        {
            if (value == null)
            {
                control.SelectedIndex = 0;
            }
            else
            {
                control.SelectedValue = value;
            }
        }

        static public string GetColumnValue(this DataGridViewRow row, string columnName)
        {
            if (row == null) return null;

            var cell = row.Cells[columnName];
            if (cell == null) return null;

            if (cell.Value == null) return null;
            return cell.Value.ToString();
        }
    }
}
