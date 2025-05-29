using LibraryManager.Models;

namespace LibraryManager.AbstractObjects;

/// <author>YR 2025-02-09</author>
public interface IBookKeeper
{
    bool TrySaveBook(Book book, string selectedPlace);
}
