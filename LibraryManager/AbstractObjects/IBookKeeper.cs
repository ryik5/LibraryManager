namespace LibraryManager.Models;
/// <author>YR 2025-01-09</author>
public interface IBookKeeper
{
    bool TrySaveBook(Book book, string selectedPlace);
}
