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

namespace System
{
    static class Extensions
    {
        static public TResult NullOr<TSource, TResult>(this TSource source, Func<TSource, TResult> func)
            where TSource : class
            where TResult : class
        {
            return source == null ? null : func(source);
        }
    }
}
