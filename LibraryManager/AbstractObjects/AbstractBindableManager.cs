using CommunityToolkit.Mvvm.Input;
using Foundation;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LibraryManager.AbstractObjects;

public abstract class AbstractBindableModel : INotifyPropertyChanged
{
    protected AbstractBindableModel()
    {
        // Initialize the generic navigation command
        NavigateExtendedCommand = new AsyncRelayCommand<string>(PerformExtendedAction);
    }
    // <summary>
    /// Displays a custom dialog page with a title and message, and returns a boolean indicating whether the user pressed OK or Cancel.
    /// </summary>
    /// <param name="title">The title of the dialog page.</param>
    /// <param name="message">The message to display on the dialog page.</param>
    /// <returns>A boolean indicating whether the user pressed OK or Cancel.</returns>
    protected async Task<ResultData> ShowCustomDialogPage(string title, string message, bool isInputVisible = false)
    {
        var dialogPage = new CustomDialogPage(title, message, isInputVisible);
        await Application.Current?.MainPage?.Navigation.PushModalAsync(dialogPage)!;

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response
        var inputText = dialogPage.InputText;

        #if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
        #endif

        return new ResultData(result, inputText);
    }
    
    public ICommand NavigateExtendedCommand { get; }

    protected virtual async Task PerformExtendedAction(string? arg1, CancellationToken arg2)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    protected void RunInMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

    // Run code in the UI thread
    // Platform-agnostic, works anywhere in MAUI
    // Slightly slower due to abstraction (but negligible)
    protected T RunInMainThread<T>(Func<T> func)
    {
        T result = default!;
        MainThread.BeginInvokeOnMainThread(() => result = func());
        return result;
    }

    // Run code in the UI thread
    // Best For -  Page- or Application-scoped logic 
    // Slightly faster in direct access situations 
    protected void RunInPageThread(Action a)
    {
        Application.Current?.Dispatcher.Dispatch(a.Invoke);
    }

    protected async Task<FileResult> TryPickFileUpTask(string windowTitle, string[]? fileExtension)
    {
        var fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                /*{ DevicePlatform.iOS, new[] { "public.archive" } },
                { DevicePlatform.Android, new[] { "application/xml" } },*/
                { DevicePlatform.WinUI,  fileExtension  },
                { DevicePlatform.MacCatalyst,  fileExtension  } //".xml", "xml"
            });
        var options = new PickOptions()
        {
            PickerTitle = windowTitle,
            FileTypes = fileExtension?.Length<1 ? null : fileTypes,
        };
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                return result;
            }
        }
        catch (Exception ex)
        {
            #if DEBUG
            Debug.WriteLine($"{ex.Message}");
            #endif
        }

        return null;
    }

    /// <summary>
    /// Gets the path to the current user Document Directory on the device.
    /// </summary>
    /// <returns>The path to the user Document Directory.</returns>
    /// <remarks>
    /// This method returns the URL of the document directory
    /// and then getting the path from the URL. The document directory is the path to the directory
    /// for storing user documents. The path returned by this method is the path to the root of this
    /// directory.
    /// </remarks>
    protected string GetPathToDocumentDirectory()
        => new NSFileManager().GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;


    /// <summary>
    /// Gets the path to a XML file in the document directory.
    /// </summary>
    /// <param name="pointedName">The name of the file to get the path for.</param>
    /// <returns>The path to the XML file in the document directory.</returns>
    protected string GetPathToFile(string pointedName)
    {
        return Path.Combine(GetPathToDocumentDirectory(), StringsHandler.CreateXmlFileName(pointedName));
    }
    /// <summary>
    /// Gets the path to a XML file in the document directory.
    /// </summary>
    /// <param name="pointedName">The name of the file to get the path for.</param>
    /// <returns>The path to the XML file in the document directory.</returns>
    protected string GetPathToFile(string pointedName, string fileExtenstion)
    {
        return Path.Combine(GetPathToDocumentDirectory(), StringsHandler.CreateXmlFileName(pointedName, fileExtenstion));
    }

    protected async Task<string> PickFolderUpTask()
    {
        return await _folderPicker.PickFolder();
    }

    
    #region implementation INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }
    #endregion
    
    private readonly IFolderPicker _folderPicker;
}

public class ResultData
{
    public ResultData(bool isOk, string? inputString = null)
    {
        IsOk = isOk;
        InputString = inputString;
    }

    public bool IsOk { get; private set; }
    public string? InputString { get; private set; }
}

public class ResultLoading
{
    public MediaData? MediaData { get; set; }
    public bool IsSuccess { get; set; }
}