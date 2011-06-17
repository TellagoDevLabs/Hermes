using System.Linq;

// ReSharper disable CheckNamespace
namespace System.Collections.Generic
// ReSharper restore CheckNamespace
{
    public static class SystemCollectionsGenericExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            if (enumeration != null)
            {
                foreach (var item in enumeration)
                {
                    action(item);
                }
            }
        }
    }
}
