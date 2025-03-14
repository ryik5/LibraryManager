using System.ComponentModel;
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
    
    
    #region implementation INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
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