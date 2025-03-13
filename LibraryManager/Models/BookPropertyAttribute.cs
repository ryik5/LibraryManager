namespace LibraryManager.Models;

/// <summary>
/// Custom attribute to mark properties that should be included in the book properties list.
/// </summary>
/// <author>YR 2025-02-24</author>
[AttributeUsage(AttributeTargets.Property)]
public class BookPropertyAttribute : Attribute
{
}