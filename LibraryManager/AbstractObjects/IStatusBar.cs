namespace LibraryManager.AbstractObjects;

public interface IStatusBar
{
    string CommonInfo { get; }

    string CurrentInfo { get; }

     string StatusInfo { get; }
     
    
    Task SetTotalBooks(int totalBooks);
    
    Task SetCurrentInfo(string message);
    
    Task SetCommonInfo(string message);
}