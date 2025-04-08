namespace LibraryManager.AbstractObjects;

public interface IStatusBar
{
    string CommonInfo { get; }

    string CurrentInfo { get; }

     string StatusInfo { get; }
     
     Task SetStatusMessage(EInfoKind infoKind, string message);

     Task SetStatusMessage(EInfoKind infoKind, int totalBooks);

}