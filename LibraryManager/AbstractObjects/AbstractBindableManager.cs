using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LibraryManager.AbstractObjects;

public abstract class AbstractBindableModel: INotifyPropertyChanged
{
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
    
    protected async Task<FileResult> TryPickFileUpTask(string windowTitle, string fileExtension)
    {
        var fileTypes = new FilePickerFileType(
            new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                /*{ DevicePlatform.iOS, new[] { "public.archive" } },
                { DevicePlatform.Android, new[] { "application/xml" } },*/
                { DevicePlatform.WinUI, new[] { $".{fileExtension}", fileExtension } },
                { DevicePlatform.MacCatalyst, new[] { $".{fileExtension}", fileExtension } } //".xml", "xml"
            });

        var options = new PickOptions()
        {
            PickerTitle = windowTitle,
            FileTypes = fileTypes,
        };
        try
        {
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                if (result.FileName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                   return result;
                }
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
}