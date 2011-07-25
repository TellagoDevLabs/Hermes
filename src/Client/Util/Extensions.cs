using System.Linq;

namespace System.Collections.Generic
{
    static class Extensions
    {
        static public void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;

            foreach (var t in source)
            {
                action(t);
            }
        }
    }

}
