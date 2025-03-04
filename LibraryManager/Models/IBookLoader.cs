﻿namespace LibraryManager.Models;

/// <author>YR 2025-01-09</author>
public interface IBookLoader : ILoadable
{
    bool TryLoadBook(string pathToBook, out Book? book);

    event EventHandler<ActionFinishedEventArgs> LoadingFinished;
}