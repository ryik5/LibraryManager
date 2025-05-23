﻿using LibraryManager.Models;

namespace LibraryManager.AbstractObjects;

/// <author>YR 2025-01-09</author>
public interface IBookLoader 
{
    bool TryLoadBook(string pathToBook, out Book? book);

    event EventHandler<ActionFinishedEventArgs> LoadingFinished;
}