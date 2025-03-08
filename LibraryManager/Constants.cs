namespace LibraryManager;

/// <summary>
/// A static class containing constants used throughout the application.
/// </summary>
/// <author>YR 2025-02-16</author>
public static class Constants
{
    #region General
    public const string ID = "ID";
    public const string LIBRARY_MANAGER = "LibraryManager";
    public const string LIBRARY_MANAGER_PIPE_SERVER = "LibraryManagerPipeServer";
    public const string OPERATION_WAS_CANCELED = "Operation was canceled";
    public const string BOOK_WAS_SAVED_SUCCESSFULLY = "Book was saved successfully";
    public const string FAILED_TO_SAVE_BOOK_TO_PATH = "Failed to save book to path";
    public const string BOOK_WAS_IMPORTED_SUCCESSFULLY = "Book was imported successfully";
    public const string FAILED_TO_IMPORT_BOOK_BY_PATH = "Failed to import book from the path";
    #endregion

    #region ToolsViewModel
    public const string TOOLS = "Tools";
    #endregion

    #region StatusBarViewModel
    public const string TOTAL_BOOKS_IN_LIBRARY = "Total books in the library";
    #endregion

    #region LibraryViewModel
    public const string LIBRARY = "Library";
    public const string CREATE_NEW_LIBRARY = "Create a new library";
    public const string LIBRARY_WAS_CREATED_SUCCESSFULLY = "Library was created successfully";
    public const string LIBRARY_LOAD = "Load the library";
    public const string LOADING_LIBRARY_FROM_XML = "Loading library from the XML file...";
    public const string LIBRARY_WAS_LOADED_SUCCESSFULLY = "Library was loaded successfully from path";
    public const string LIBRARY_LOADED_WITH_ID = "Library loaded with ID";
    public const string LIBRARY_WAS_NOT_LOADED = "Library was not loaded";
    public const string FAILED_TO_LOAD_LIBRARY_FROM_PATH = "Failed to load library from path";
    public const string LIBRARY_LOADING_FINISHED = "Library loading finished";
    public const string LIBRARY_SAVE = "Save Library";
    public const string LIBRARY_SAVE_WITH_NEW_NAME = "Save the library with a new name";
    public const string FOLDER_WAS_NOT_SELECTED = "Folder was not selected";
    public const string LIBRARY_IS_BEING_SAVED = "The library is being saved...";
    public const string LIBRARY_WAS_SAVED_SUCCESSFULLY = "Library was saved successfully to path";
    public const string FAILED_TO_SAVE_LIBRARY_TO_PATH = "Failed to save library to path";
    public const string LIBRARY_CLOSE = "Close the current library";
    public const string LIBRARY_WAS_CLOSED = "Library was closed";
    public const string INPUT_NEW_NAME_LIBRARY = "Input the new name of the library:";
    public const string LIBRARY_NAME = "Library name";
    #endregion

    #region book details: FindBookViewModel, CreatorBookDetailsViewModel, EditorBookDetailsViewModel
     public const string ADD_BOOK = "Add Book";
     public const string DEMO_ADD_BOOKS = "Demo add books";
     public const string LAST_ADDED_BOOK = "Last added book";
     public const string ADDING_BOOK_WAS_CANCELLED = "Adding book was canceled";
     public const string EDIT_BOOK = "Edit Book";
     public const string ORIGINAL_STATE_BOOK = "Original state of the book";
     public const string LOADED_FOR_EDITING = "is loaded for editing";
     public const string LAST_EDITED_BOOK = "Last edited book";
     public const string EDITING_BOOK_WAS_CANCELLED = "Editing book was canceled";
 
    public const string FIND_BOOKS = "Find Books";
    public const string SORT_BOOKS = "Sort Books";
    public const string SORTING_ASCENDING = "a...Z";
    public const string SORTING_DESCENDING = "Z...a";
    public const string DELETE_BOOK = "Delete the selected book";
    public const string BOOK_WAS_DELETED_SUCCESSFULLY = "Book was deleted successfully";
    public const string NO_BOOKS_FOUND = "No books found";
    public const string CONTENT_WAS_LOADED = "Content was loaded";
    public const string LOAD_CONTENT = "Load content";
    public const string LOADING_STARTED = "Loading started";
    public const string NEW_CONTENT_WAS_LOADED_INTO_BOOK = "New content was loaded into the book";
    public const string CONTENT_WAS_NOT_LOADED_SUCCESSFULLY = "The content was not loaded successfully";
    public const string ATTEMPT_TO = "Attempt to";
    public const string SAVE_CHANGES = "Save changes";
    public const string SAVE_CONTENT = "save content";
    public const string SAVING_STARTED = "Saving started";
    public const string CONTENT_SAVED_SUCCESSFULLY = "Content saved successfully";
    public const string FAILED_TO_SAVE_CONTENT = "Failed to save content";
    public const string NO_CONTENT_TO_SAVE = "No content to save";
    public const string IMPORT_BOOK = "Import a book";
    public const string EXPORT_BOOK = "Export the selected book";
    public const string INPUT_BOOK_NAME = "Input a name of the book:";
    public const string INPUT_NAME = "Input the name";
    #endregion

    #region DebugViewModel
    public const string DEBUG = "Debug";
    #endregion

    #region BooksViewModel
    public const string BOOKS = "Books";
    #endregion

    #region AboutViewModel
    public const string ABOUT = "About";
    #endregion
}