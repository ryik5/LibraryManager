using System.Collections.ObjectModel;

namespace LibraryManager.Extensions;

/// <summary>
/// Provides extension methods for working with collections.
/// </summary>
/// <author>YR 2025-02-06</author>
internal static class CollectionExtentions
{
    /// <summary>
    /// Removes the specified item from the target ObservableCollection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the ObservableCollection.</typeparam>
    /// <param name="target">The ObservableCollection to remove the item from.</param>
    /// <param name="item">The item to remove.</param>
    /// <returns>True if the item was removed, false otherwise.</returns>
    public static bool RemoveItem<T>(this ObservableCollection<T> target, T item)
    {
        if (target is null || item is null)
            return false;

        var index = target.IndexOf(item);

        if (index >= 0 && index < target.Count)
        {
            target.RemoveAt(index);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Resets the specified ICollection and adds a range of items.
    /// </summary>
    /// <typeparam name="T">The type of elements in the ICollection.</typeparam>
    /// <param name="collection">The ICollection to reset and add items to.</param>
    /// <param name="sourceNewItems">The IEnumerable of items to add.</param>
    public static void ResetAndAddRange<T>(this ICollection<T> collection, IEnumerable<T> sourceNewItems)
    {
        if (collection is null || sourceNewItems == null)
            return;
        var list = sourceNewItems?.ToList() ?? new List<T>(0);
        collection.Clear();
        list.ForEach(item =>
        {
            if (item != null)
                collection.Add(item);
        });
    }
}