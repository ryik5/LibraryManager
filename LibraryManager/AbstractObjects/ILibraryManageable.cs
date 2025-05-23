﻿using LibraryManager.Models;

namespace LibraryManager.AbstractObjects;

/// <summary>
/// Represents a library interface that provides functionalities to add, remove, sort, and display Library.
/// </summary>
/// <author>YR 2025-01-09</author>
public interface ILibraryManageable 
{
    public event EventHandler<ActionFinishedEventArgs> LoadingFinished;

    /// <summary>
    /// Executes a specified command within the context of book management.
    /// </summary>
    /// <param name="commandParameter">The parameter or name of the command to execute.</param>
    /// <remarks>
    /// This method provides a way to run specific functions or operations using a command parameter,
    /// enabling additional extensibility or dynamic behaviors based on the provided input.
    /// </remarks>
    Task RunCommand(string commandParameter);
    
    /// <summary>
    /// Creates a new library with the specified ID.
    /// </summary>
    Task CreateNewLibrary();

    /// <summary>
    /// Loads a library from the specified path.
    /// </summary>
    /// <param name="libraryLoader">The loader responsible for loading the library.</param>
    /// <param name="pathToLibrary">The path to the file containing the library data.</param>
    /// <returns>True if the library was successfully loaded; otherwise, false.</returns>
    bool TryLoadLibrary(ILibraryLoader libraryLoader, string pathToLibrary);

    /// <summary>
    /// Loads a library.
    /// </summary>
    Task<bool> TryLoadLibrary();
    
    /// <summary>
    /// Saves the specified library to the specified folder.
    /// </summary>
    /// <param name="keeper">The keeper responsible for saving the library.</param>
    /// <param name="pathToStorage">The path to the storage where the library will be saved.</param>
    /// <returns>True if the library was successfully saved; otherwise, false.</returns>
    Task <bool> TrySaveLibrary(ILibraryKeeper keeper, string pathToStorage);

    /// <summary>
    /// Closes the library and clears the book list.
    /// </summary>
    Task TryCloseLibrary();

    /// <summary>
    /// Gets or sets the library.
    /// </summary>
    ILibrary? Library
    {
        get; set;
    }
}
