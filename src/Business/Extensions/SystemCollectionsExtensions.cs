using System.Linq;

// ReSharper disable CheckNamespace
namespace System.Collections
// ReSharper restore CheckNamespace
{
    public static class SystemCollectionsExtensions
    {
        public static void ForEach<T>(this IEnumerable enumeration, Action<T> action)
        {
            if (enumeration != null)
            {
                enumeration
                    .Cast<T>()
                    .ForEach(action);
            }
        }
    }
}
