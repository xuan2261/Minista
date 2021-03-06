﻿using System.Linq;

namespace System.Collections.Generic
{
    /// <summary>
    /// An extension <see langword="class"/> for the <see cref="System.Collections.Generic"/> <see langword="namespace"/>
    /// </summary>
    internal static class GenericExtensions
    {
        /// <summary>
        /// Merges the two input <see cref="IReadOnlyDictionary{TKey,TValue}"/> instances and makes sure no duplicate keys are present
        /// </summary>
        /// <param name="a">The first <see cref="IReadOnlyDictionary{TKey,TValue}"/> to merge</param>
        /// <param name="b">The second <see cref="IReadOnlyDictionary{TKey,TValue}"/> to merge</param>
        public static IReadOnlyDictionary<TKey, TValue> Merge<TKey, TValue>(
             this IReadOnlyDictionary<TKey, TValue> a,
             IReadOnlyDictionary<TKey, TValue> b)
        {
            if (a.Keys.FirstOrDefault(b.ContainsKey) is TKey key) throw new InvalidOperationException($"The key {key} already exists in the current pipeline");

            var c = a.Concat(b);
            var l = new Dictionary<TKey, TValue>();
            for (int i = 0; i < c.Count(); i++)
                l.Add(c.ElementAt(i).Key, c.ElementAt(i).Value);
            return l;
        }

        /// <summary>
        /// Merges the two input <see cref="IReadOnlyCollection{T}"/> instances and makes sure no duplicate items are present
        /// </summary>
        /// <param name="a">The first <see cref="IReadOnlyCollection{T}"/> to merge</param>
        /// <param name="b">The second <see cref="IReadOnlyCollection{T}"/> to merge</param>
        public static IReadOnlyCollection<T> Merge<T>( this IReadOnlyCollection<T> a,  IReadOnlyCollection<T> b)
        {
            if (a.Any(b.Contains)) throw new InvalidOperationException("The input collection has at least an item already present in the second collection");
            return a.Concat(b).ToArray();
        }
    }
}
