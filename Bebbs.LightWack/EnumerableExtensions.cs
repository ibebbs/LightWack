using System;
using System.Collections.Generic;

namespace Bebbs.LightWack
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);

                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T item in source)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
        {
            int index = 0;
            IEnumerator<T> enumerator = source.GetEnumerator();

            while (enumerator.MoveNext())
            {
                action(index, enumerator.Current);
                index++;
            }
        }
    }
}
