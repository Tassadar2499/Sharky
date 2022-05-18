using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharky._Common;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> elements)
    {
        foreach (var element in elements)
            collection.Add(element);
    }

    public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable == null)
            return true;

        return enumerable.Any() is false;
    }
}