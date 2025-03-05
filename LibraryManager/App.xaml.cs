using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.Models;
using LibraryManager.ViewModels;

namespace LibraryManager;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        Services = serviceProvider;

        // Configure dependency injection
        /*var serviceProvider = new ServiceCollection()
            .AddSingleton<LibraryViewModel>()
            .AddSingleton<BooksViewModel>()
            .BuildServiceProvider();

        Services = serviceProvider;*/


      //  SetLibrary();

        MainPage = new AppShell();

        // Publisher - example
        WeakReferenceMessenger.Default.Send(new ReferenceMessage("sender_id", "Hello from sender"));

        // Subscriber - example
        WeakReferenceMessenger.Default.Register<ReferenceMessage>(this, (recipient, message) =>
        {
            Debug.WriteLine($"Message received from: {message.SenderID}");
            Debug.WriteLine($"Message content: {message.Value}");
        });
    }

    public static IServiceProvider Services { get; private set; }

    public SettingsViewModel Settings { get; } = new SettingsViewModel();

    public void SetLibrary()
    {
        Library = new Library
        {
            Id = new Random().Next(),
            Name = "",
            Description = "",
            // Populate with sample data       
            BookList = new ObservableCollection<Book>
            {
                new Book { Id = 0, Title = "1984", Author = "George Orwell", Year = 1949, TotalPages = 1 },
                new Book { Id = 1, Title = "Pride and Prejudice", Author = "Jane Austen", Year = 1813, TotalPages = 1 },
                new Book
                {
                    Id = 2, Title = "The Catcher in the Rye", Author = "J.D. Salinger", Year = 1951, TotalPages = 1
                }
            }
        };
    }

    public ILibrary Library { get; private set; }


    public LibraryViewModel LibraryViewModel { get;  } 

    public BooksViewModel BooksViewModel { get; }
}