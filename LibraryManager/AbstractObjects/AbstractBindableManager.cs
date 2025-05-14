using Foundation;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LibraryManager.AbstractObjects;

/// <summary>
/// Abstract base class for bindable models, providing common functionality for data binding and UI interactions.
/// </summary>
/// <author>YR 2025-03-09</author>
public abstract class AbstractBindableModel : INotifyPropertyChanged
{
    #region Public Properties
    /// <summary>
    /// Command to perform an action, such as navigate to a different page or view or other actions.
    /// </summary>
    public ICommand NavigateCommand { get; set; }

    /// <summary>
    /// Gets or sets the status bar instance.
    /// </summary>
    public IStatusBar StatusBar
    {
        get => _statusBar;
        set => SetProperty(ref _statusBar, value);
    }
    #endregion


    #region Public Methods
    /// <summary>
    /// Displays a custom dialog page with a title and message, and returns a boolean indicating whether the user pressed OK or Cancel.
    /// </summary>
    /// <param name="title">The title of the dialog page.</param>
    /// <param name="message">The message to display on the dialog page.</param>
    /// <param name="isInputVisible">Whether the input field is visible.</param>
    /// <returns>A boolean indicating whether the user pressed OK or Cancel.</returns>
    protected async Task<ResultInput> ShowCustomDialogPage(string title, string message, bool isInputVisible = false)
    {
        var dialogPage = new CustomDialogPage(title, message, isInputVisible);
        await Application.Current?.MainPage?.Navigation.PushModalAsync(dialogPage)!;

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response
        var inputText = dialogPage.InputText;

        #if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
        #endif

        return new ResultInput(result, inputText);
    }
    protected async Task<ResultInput> ShowCustomDialogPage(string title, string message, bool isInputVisible=true, string inputStartText="InputHere")
    {
        var dialogPage = new CustomDialogPage(title, message, isInputVisible, inputStartText);
        await Application.Current?.MainPage?.Navigation.PushModalAsync(dialogPage)!;

        var result = await dialogPage.DialogResultTask.Task; // Await the user's response
        var inputText = dialogPage.InputText;

        #if DEBUG
        Debug.WriteLine(result ? "User pressed OK." : "User pressed Cancel.");
        #endif

        return new ResultInput(result, inputText);
    }

    protected async Task ShowDisplayPromptAsync(string message)
    {
        await Application.Current.MainPage.DisplayPromptAsync("Error", message);
    }


    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    protected void RunInMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);


    /// <summary>
    /// Invokes the specified action on the UI thread.
    /// </summary>
    /// <param name="action">The action to invoke.</param>
    protected async Task RunInMainThreadAsync(Action action) => await Task.Run(() => RunInMainThread(action));


    /// <summary>
    /// Runs code in the UI thread and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="func">The function to invoke.</param>
    /// <returns>The result of the function.</returns>
    ///<remarks>
    /// Run code in the UI thread
    /// Platform-agnostic, works anywhere in MAUI
    /// Slightly slower due to abstraction (but negligible)
    /// </remarks>
    protected T RunInMainThread<T>(Func<T> func)
    {
        T result = default!;
        MainThread.BeginInvokeOnMainThread(() => result = func());
        return result;
    }


    /// <summary>
    /// Runs code in the UI thread, best for page- or application-scoped logic.
    /// </summary>
    /// <param name="a">The action to invoke.</param>
    /// <remarks>
    /// Run code in the UI thread
    /// Best For -  Page- or Application-scoped logic
    ///  Slightly faster in direct access situations 
    /// </remarks>
    protected void RunInPageThread(Action a)
    {
        Application.Current?.Dispatcher.Dispatch(a.Invoke);
    }


    /// <summary>
    /// Attempts to pick a file up from the file system.
    /// </summary>
    /// <param name="windowTitle">The title of the file picker window.</param>
    /// <param name="fileExtension">The file extension to filter by.</param>
    /// <returns>The picked file result.</returns>
    protected async Task<FileResult> TryPickFileUpTask(string windowTitle, string[]? fileExtension)
    {
        var fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                /*{ DevicePlatform.iOS, new[] { "public.archive" } },
                { DevicePlatform.Android, new[] { "application/xml" } },*/
                { DevicePlatform.WinUI, fileExtension },
                { DevicePlatform.MacCatalyst, fileExtension } //".xml", "xml"
            });
        var options = new PickOptions()
        {
            PickerTitle = windowTitle,
            FileTypes = fileExtension?.Length < 1 ? null : fileTypes,
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
    /// Gets the path to an XML file in the document directory.
    /// </summary>
    /// <param name="pointedName">The name of the file to get the path for.</param>
    /// <returns>The path to the XML file in the document directory.</returns>
    protected string GetPathToFile(string pointedName, string fileExtenstion)
    {
        return Path.Combine(GetPathToDocumentDirectory(),
            StringsHandler.CreateXmlFileName(pointedName, fileExtenstion));
    }
    #endregion


    #region Implementation INotifyPropertyChanged
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


    private IStatusBar _statusBar;
}