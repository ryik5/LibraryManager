namespace LibraryManager.Extensions;

/// <summary>
/// Provides extension methods for working with enumerations.
/// </summary>
/// <author>YR 2025-02-27</author>
public static class EnumExtensions
{
    /// <summary>
    /// Returns the next enumeration value in the sequence.
    /// </summary>
    /// <typeparam name="T">The type of the enumeration.</typeparam>
    /// <param name="src">The current enumeration value.</param>
    /// <returns>The next enumeration value in the sequence.</returns>
    /// <exception cref="ArgumentException">Thrown if the type is not an enumeration.</exception>
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) 
            throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        var j = Array.IndexOf<T>(Arr, src) + 1;
        return (Arr.Length == j) ? Arr[0] : Arr[j];
    }
}
