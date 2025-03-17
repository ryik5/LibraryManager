using LibraryManager.Models;
using System.Diagnostics;

namespace LibraryManager.ViewModels;

public class EditBookViewModel : AbstractViewModel
{
    private Book _book;

    public EditBookViewModel(Book book)
    {
        Book = book;
    }

    #region Public Properties
    public Book Book
    {
        get => _book;
        set => SetProperty(ref _book, value);
    }
    #endregion


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        Debug.WriteLine($"NavigateCommand triggered with commandParameter: {commandParameter}");

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        // Prevent navigation to the same page - 'Shell.Current.GoToAsync(...'
        if (IsCurrentRoute(nameof(EditBookPage)))
        {
            switch (commandParameter)
            {
                case Constants.SAVE_CONTENT:
                    break;

                case Constants.LOAD_CONTENT:
                    break;

                default:
                {
                    // Settings view
                    // Debug view
                    break;
                }
            }
        }
        else
        {
            await ShowDebugNavigationError(commandParameter, nameof(EditBookViewModel));
        }
    }
    #endregion
}