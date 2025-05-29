using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.AbstractObjects;
using LibraryManager.Models;
using LibraryManager.Utils;
using LibraryManager.Views;

namespace LibraryManager.ViewModels;

/// <author>YR 2025-02-09</author>
public sealed partial class LibraryViewModel : AbstractBookViewModel, IRefreshable
{
    public LibraryViewModel(ILibrary library, SettingsViewModel settings, IStatusBar statusBar)
        : base(library, statusBar)
    {
        _settings = settings;
        _libraryManager = new LibraryManagerModel(Library, statusBar);
    }


    #region Public Methods
    protected override async Task PerformAction(string? commandParameter)
    {
        #if DEBUG
        await ShowNavigationCommandInDebug(commandParameter, nameof(LibraryPage));
        #endif

        if (string.IsNullOrWhiteSpace(commandParameter))
            return;

        if (IsCurrentRoute(nameof(LibraryPage)))
        {
            switch (commandParameter)
            {
                case nameof(AboutPage):
                case nameof(BooksPage):
                case nameof(FindBooksPage):
                case nameof(ToolsPage):
                {
                    await TryGoToPage(commandParameter);
                    break;
                }
                case Constants.LIBRARY_NEW:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        if (await ShowSelectorPopupAsync(
                                "Library has been changed but it isn't saved.\nDo you want to save library?"))
                        {
                            WeakReferenceMessenger.Default.Send(new StatusMessage()
                            {
                                InfoKind = EInfoKind.CurrentInfo,
                                Message = "Library hasn't been closed. You must save it first."
                            });

                            return;
                        }
                    }

                    await _libraryManager.CreateNewLibrary(); 
                    await UpdateLibraryHashCode();
                    
                    break;
                }
                case Constants.LIBRARY_LOAD:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        var shouldSave = await ShowCustomDialogPage(Constants.LIBRARY_SAVE,
                            StringsHandler.LibraryChangedMessage(), false);
                        if (shouldSave.IsOk)
                        {
                            var shouldSaveLibrary = await ShowCustomDialogPage(Constants.SAVE_CHANGES,
                                Constants.LIBRARY_SAVE_WITH_NAME, true, Library.Id.ToString());
                            bool success = true;
                            if (shouldSaveLibrary.IsOk)
                                success =
                                    await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(),
                                        GetPathToCurrentLibraryFile(shouldSaveLibrary.InputString));

                            if (!success)
                                return;
                        }
                    }

                    // await  TryGoToPage(nameof(LibraryPage));
                    var isLoaded = await _libraryManager.TryLoadLibrary();
                     if (isLoaded)
                        Library.IsNew = false;

                    await UpdateLibraryHashCode();
                    await HandlePostRefreshControlsOnAppearingTask();
                    var msg = isLoaded ? $"Loaded the Library with ID:{Library.Id}" : $"Error loading the Library";
                   WeakReferenceMessenger.Default.Send(new StatusMessage()
                        { InfoKind = EInfoKind.CurrentInfo, Message = msg });
                    break;
                }
                case Constants.LIBRARY_SAVE:
                {
                    var res = await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(),
                        GetPathToCurrentLibraryFile());
                    if (res)
                    {
                        Library.IsNew = false;
                        await UpdateLibraryHashCode();
                    }

                    var msg = res
                        ? $"Library with ID:{Library.Id} saved successfully."
                        : $"Error saving the Library with ID:{Library.Id}";
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.CurrentInfo,
                        Message = msg
                    });

                    break;
                }
                case Constants.LIBRARY_SAVE_WITH_NAME:
                {
                    // await ShowPopUpView(popup);
                    //https://www.nuget.org/packages/Mopups/
                    var inputtedLibraryName = await ShowInputPopupAsync($"{Library.Id}",$"Library_name"  );
                     var isInputted =!Constants.NoText.Equals(inputtedLibraryName);
                    var libName = isInputted && !string.IsNullOrEmpty(inputtedLibraryName) ? inputtedLibraryName : Library.Id.ToString();
                    
                    var res = await _libraryManager.TrySaveLibrary(new XmlLibraryKeeper(), GetPathToFile(libName));
                    if (res)
                        await UpdateLibraryHashCode();
                    var msg = res
                        ? $"Library with ID:'{Library.Id}' saved on disk with name '{libName}' successfully."
                        : $"Error Of saving the Library with ID:'{Library.Id}' as '{libName}'";
                    WeakReferenceMessenger.Default.Send(new StatusMessage()
                    {
                        InfoKind = EInfoKind.CurrentInfo,
                        Message = msg
                    });
                    break;
                }
                case Constants.LIBRARY_CLOSE:
                {
                    if (await HasLibraryHashCodeChanged())
                    {
                        if (await ShowSelectorPopupAsync(
                                "Library has been changed but it isn't saved.\nDo you want to save library?"))
                        {
                            WeakReferenceMessenger.Default.Send(new StatusMessage()
                            {
                                InfoKind = EInfoKind.CurrentInfo,
                                Message = "Library hasn't been closed. You must save it first."
                            });

                            return;
                        }
                    }

                    await _libraryManager.TryCloseLibrary();
                    await UpdateLibraryHashCode();

                    await HandleCloseLibrary();
                    break;
                }
                default:
                {
                    // Performs other actions at the LibraryManager
                    await _libraryManager.RunCommand(commandParameter);
                    break;
                }
            }
        }
        else
        {
            #if DEBUG
            await ShowNavigationErrorInDebug(commandParameter, nameof(FindBooksViewModel));
            #endif
        }
    }


    protected override async Task HandlePostRefreshControlsOnAppearingTask()
    {
        await RunInMainThreadAsync(() =>
        {
            Library.TotalBooks = Library.BookList.Count;
            CanOperateWithBooks = IsValidLibrary && IsNotEmptyLibrary;
            CanCloseLibrary = IsValidLibrary;
        });
    }

    private async Task HandleCloseLibrary()
    {
        await Task.Delay(10);
        CanOperateWithBooks = false;
        CanCloseLibrary = false;
    }
    #endregion

    #region Public Properties
    /// <summary>
    /// Gets or sets a value indicating whether the book can be edited.
    /// </summary>
    [ObservableProperty] private bool _canCloseLibrary;
    #endregion

    #region Private methods
    /// <summary>
    /// Updates the library state by raising a property changed event for the Library property
    /// and setting the IsEnabled property based on whether the Library ID differs from the default value of 0.
    /// </summary>
    private Task UpdateLibraryHashCode()
    {
        _libraryHashCode = Library.GetHashCode();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Checks if the library hash code has changed and prompts the user to save changes if necessary.
    /// </summary>
    /// <returns>True if the user confirms saving changes, false otherwise.</returns>
    private Task<bool> HasLibraryHashCodeChanged()
    {
        if (!IsValidLibrary || IsEmptyLibrary&&Library.IsNew)
            return Task.FromResult(false); 

        var currentHash = Library.GetHashCode();
        return Task.FromResult(currentHash != 0 && currentHash != _libraryHashCode);

       // if (libraryChanged)
        {
          //  var res = await ShowSelectorPopupAsync(StringsHandler.LibraryChangedMessage());
          //  return res;

            // res = await ShowCustomDialogPage(Constants.LIBRARY_SAVE, StringsHandler.LibraryChangedMessage(), false);
            // return res.IsOk;
        }

        //return false;
    }


    private string GetPathToCurrentLibraryFile(string? libName = null) =>
        GetPathToFile(libName ?? Library.Id.ToString());
    #endregion

    #region Private Members
    private readonly SettingsViewModel _settings;
    private readonly ILibraryManageable _libraryManager;
    private int _libraryHashCode;
    #endregion
}