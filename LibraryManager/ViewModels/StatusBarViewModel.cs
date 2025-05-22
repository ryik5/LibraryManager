using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using System.Collections.Concurrent;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-03-09</author>
public sealed partial class StatusBarViewModel : AbstractBindableModel, IStatusBar
{
    public StatusBarViewModel()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        WeakReferenceMessenger.Default.Register<StatusMessage>(this,
            (r, m) => { Task.Run(() => _messages.Enqueue(m), _cancellationTokenSource.Token); });
        ReadStatusMessageFromQueueTask(_cancellationTokenSource).GetAwaiter();
    }

    #region Public Properties
    /// <summary>
    /// General information about the application or system.
    /// </summary>
    [ObservableProperty] string _commonInfo;

    /// <summary>
    /// Information about the current state or operation being performed.
    /// </summary>
    [ObservableProperty] private string _currentInfo;

    /// <summary>
    /// Total number of books in the current library.
    /// </summary>
    [ObservableProperty] string _totalBooksInfo;

    /// <summary>
    /// Detailed information for debugging purposes, typically used for troubleshooting.
    /// </summary>
    [ObservableProperty] private List<IndexedString> _debugInfo = new();
    #endregion

    #region Private Methods
    private async Task SetStatusMessageTask(StatusMessage message)
    {
        switch (message.InfoKind)
        {
            case EInfoKind.CommonInfo:
                await SetCommonInfo(message.Message);
                break;
            case EInfoKind.CurrentInfo:
                await SetCurrentInfo(message.Message);
                break;
            case EInfoKind.TotalBooks:
                message.Message = $"Total book(s): {message.Message}";
                await SetTotalBooks(message.Message);
                break;
        }

        await SetDebugInfo(message);
    }

    private Task ReadStatusMessageFromQueueTask(CancellationTokenSource cts)
    {
        // Run the task on a separate thread.
        return Task.Run(async () =>
        {
            // Loop until the cancellation token is cancelled.
            while (!cts.IsCancellationRequested)
            {
                if (_messages.TryDequeue(out var msg))
                {
                    await SetStatusMessageTask(msg);
                }

                // Sleep for 200 milliseconds.
                await Task.Delay(200);
            }
        }, cts.Token);
    }

    private Task SetTotalBooks(string message)
    {
        TotalBooksInfo = message;
        return Task.CompletedTask;
    }

    private Task SetCurrentInfo(string message)
    {
        CurrentInfo = message;
        return Task.CompletedTask;
    }

    private Task SetCommonInfo(string message)
    {
        CommonInfo = message;
        return Task.CompletedTask;
    }

    private Task SetDebugInfo(StatusMessage message)
    {
        DebugInfo.Add(new IndexedString
        {
            TimeStamp = $"{DateTime.Now:HH:mm:ss:fff}",
            LogLevel = message.LogLevel,
            Message = message.Message
        });
        return Task.CompletedTask;
    }
    #endregion
    
    #region Private fields
    private readonly CancellationTokenSource _cancellationTokenSource;
    private static readonly ConcurrentQueue<StatusMessage> _messages = new();
    #endregion
}