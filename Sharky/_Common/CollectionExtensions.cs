using System.Collections.Generic;

namespace Sharky._Common;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
    {
        foreach (var element in elements)
            collection.Add(element);
    }
}