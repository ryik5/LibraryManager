using LibraryManager.Models;

namespace LibraryManager.Utils;

/// <summary>
/// A utility class for generating book data.
/// </summary>
/// <author>YR 2025-02-03</author>
internal static class BookModelMaker
{
    /// <summary>
    /// Generates a book with randomly generated properties.
    /// </summary>
    /// <returns>A new instance of Book with random properties.</returns>
    public static Book GenerateDemoBook()
    {
        var id = Random.Shared.Next();
        var title = GenerateTitle();
        var author = GenerateAuthor();
        var year = GetRandomInt(1934, 2025, generatedYears);
        var pages = GetRandomInt(1, 777, generatedPages);

        return new Book()
        {
            Id = id,
            Author = author,
            Title = title,
            TotalPages = pages,
            Year = year
        };
    }

    /// <summary>
    /// Generates a book with empty generated properties.
    /// </summary>
    /// <returns>A new instance of Book.</returns>
    public static Book GenerateBook()
    {
        var id = Random.Shared.Next();
        var title = String.Empty;
        var author = String.Empty;
        var year = DateTime.Now.Year;
        var pages = 0;

        return new Book()
        {
            Id = id,
            Author = author,
            Title = title,
            TotalPages = pages,
            Year = year
        };
    }

    #region private methods
    /// <summary>
    /// Generates a random book title.
    /// </summary>
    /// <returns>A unique book title.</returns>
    private static string GenerateTitle()
    {
        string title;
        do
        {
            var numTitles = random.Next(2, 4);
            var titles = new string[numTitles];

            for (var i = 0; i < numTitles - 1; i++)
            {
                titles[i] = random.Next(2) == 0 ? GetRandomElement(title1) : GetRandomElement(title2);
            }

            titles[numTitles - 1] = GetRandomElement(title3);
            title = string.Join(GetRandomElement(titleCombining), titles);
        }
        while (generatedTitles.Contains(title));

        generatedTitles.Add(title);

        if (generatedTitles.Count > totalVariations)
            generatedTitles.RemoveAt(0);

        return title;
    }

    /// <summary>
    /// Generates a random book author.
    /// </summary>
    /// <returns>A unique book author.</returns>
    private static string GenerateAuthor()
    {
        string author;
        do
        {
            var author1Part = GetRandomElement(author1);
            var author2Part = GetRandomElement(author2);

            author = $"{author2Part} {author1Part}";
        }
        while (generatedAuthors.Contains(author));

        generatedAuthors.Add(author);

        if (generatedAuthors.Count > totalVariations)
            generatedAuthors.RemoveAt(0);

        return author;
    }

    /// <summary>
    /// Retrieves a random element from the specified array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to retrieve an element from.</param>
    /// <returns>A random element from the array.</returns>
    private static T GetRandomElement<T>(T[] array)
    {
        return array[random.Next(array.Length)];
    }

    /// <summary>
    /// Generates a random integer within the specified range.
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <param name="generatedInt">A list of previously generated integers.</param>
    /// <returns>A unique random integer within the specified range.</returns>
    private static int GetRandomInt(int min, int max, List<int> generatedInt)
    {
        int expectedInt;
        do { expectedInt = random.Next(min, max); }
        while (generatedInt.Contains(expectedInt));

        generatedInt.Add(expectedInt);

        if (generatedInt.Count > totalVariations)
            generatedInt.RemoveAt(0);

        return expectedInt;
    }
    #endregion

    #region constants and dictionaries
    private static readonly string[] titleCombining = new string[] {". ", ": ", " - "};

    private static readonly string[] author1 =new string[]  {
       "McConnell",
        "Hunt",
        "Thomas",
        "McConnell",
        "Riabchenko",
        "Twain",
        "Yavorska",
        "Adams",
        "Bradbury",
        "Haddon",
        "K. Dick",
        "Lee",
        "Kundera",
        "Rankin",
        "Shevchenko",
        "Potter",
        "Hagrid",
        "Granger",
        "Weasley",
        "Voldemort",
        "Dumbledore",
        "Snape"
   };
    private static readonly string[] author2 = new string[] {
        "Steve",
        "Andrew",
        "David",
        "Steve",
        "Iurii",
        "Mark",
        "Illa",
        "Douglas",
        "Ray",
        "Mark",
        "Philip",
        "Harper",
        "Milan",
        "Robert",
        "Taras",
        "Harry",
        "Rubeus",
        "Granger",
        "Ron",
        "Lord",
        "Albus",
        "Severus"
    };
    private static readonly string[] title1 = new string[] {
         "Against Method",
        "Total Efficiency",
        "Practical Strategies",
        "The Cooperative Game",
        "Software Writing",
        "Wild Software",
        "Debugging the Development Process",
        "Survival Guide",
        "Computing Calamities",
        "Software Craftsmanship",
        "Writing Excellent Code",
        "Computer Vision",
        "Commonsense Reasoning",
        "Computer Algebra",
        "Computer Explorations of Fractals",
        "Hogwarts School of Witchcraft and Wizardry",
        "The Philosopher's Stone",
        "The Deathly Hallows",
        "The Chamber of Secrets",
        "The Goblet of Fire",
        };
    private static readonly string[] title2 = new string[] {
        "Clean Code",
        "Code Complete",
        "Getting Real",
        "Perfect Software",
        "Software Engineering",
        "Software Estimation",
        "Agile Software Development",
        "Software Project Survival Guide",
        "Building Solid",
        "Rapid Development",
        "The Annotated Turing",
        "Data Structures",
        "The Little Schemer",
        "The Art of Computer Programming",
        "Proofs and Refutations"
    };
    private static readonly string[] title3 = new string[] {
        "C#",
        "Java",
        "C++",
        "Python",
        "Visual Basic",
        "Visual Studio",
        "Cobol",
        "Fortran",
        "Javascript",
        "HTML",
        "Internet",
        "CSS",
        ".NET",
        "Unity",
        "Android",
        "Swift",
        "The Cursed Child"
        };
    #endregion

    #region private fields
    private const int totalVariations = 50;
    private static readonly Random random = new();
    private static readonly List<string> generatedAuthors = new();
    private static readonly List<string> generatedTitles = new();
    private static readonly List<int> generatedYears = new();
    private static readonly List<int> generatedPages = new();
    #endregion
}
