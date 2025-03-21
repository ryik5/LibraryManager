using LibraryManager.Models;
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
                { DevicePlatform.WinUI, new[] { fileExtension } },
                { DevicePlatform.MacCatalyst, new[] { fileExtension } } //".xml", "xml"
            });
/*FilePickerFileType customFileType = new FilePickerFileType(
   new Dictionary<DevicePlatform, IEnumerable<string>>
   {
                   { DevicePlatform.iOS, new[] { "public.audio" } }, // UTType values
                   { DevicePlatform.Android, new[] { "audio/*" } }, // MIME type
                   { DevicePlatform.WinUI, new[] { ".mp3", ".wav", ".wma", ".m4a", ".flac" } }, // file extension
                   { DevicePlatform.Tizen, new[] { "* /*" } },
                   { DevicePlatform.macOS, new[] { ".mp3", ".wav", ".wma", ".m4a", ".flac" } }, // UTType values
   });*/
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
    
    public async Task<bool> TryLoadContentToBookTask(Book book)
    {
        var fileResult = await TryPickFileUpTask("Select book content", "xml");
        var readContentTask = await ReadContentFromDiskTask(fileResult, maxContentLength: 1000000);
        if (readContentTask.IsSuccess)
        {
            book.Content = readContentTask.MediaData;
            return true;
        }

        return false;
    }

    public async Task<ResultLoading> ReadContentFromDiskTask(FileResult fileResult, long maxContentLength)
    {
        var fileInfo = new FileInfo(fileResult.FileName);
        var media = new MediaData
        {
            Name = fileInfo.Name,
            OriginalPath = fileResult.FullPath,
            Ext = fileInfo.Extension,
            IsContentStoredSeparately = true,
            IsLoaded = false
        };
        try
        {
            var stream = await fileResult.OpenReadAsync();
            if (stream.Length < maxContentLength)
            {
                media.ObjectByteArray = await ConvertStreamToByteArray(stream);
                media.IsLoaded = true;
                media.IsContentStoredSeparately = false;
            }
            else
            {
                media.IsLoaded = true;
                media.IsContentStoredSeparately = true;
            }

            return new ResultLoading() { MediaData = media, IsSuccess = true };
        }
        catch
        {
            media.ObjectByteArray = null;
        }


        return new ResultLoading() { MediaData = null, IsSuccess = false };
    }

    protected Task<byte[]> ConvertStreamToByteArray(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return Task.FromResult(ms.ToArray());
        }
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

public  class ResultLoading
{
    public MediaData? MediaData { get; set; }
    public bool IsSuccess { get; set; }
}