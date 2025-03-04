using CommunityToolkit.Mvvm.Messaging;
using LibraryManager.Models;

namespace LibraryManager;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();


        // Publisher - example
        WeakReferenceMessenger.Default.Send(new ReferenceMessage("sender_id", "Hello from sender"));

        // Subscriber - example
        WeakReferenceMessenger.Default.Register<ReferenceMessage>(this, (recipient, message) =>
        {
            Console.WriteLine($"Message received from: {message.SenderID}");
            Console.WriteLine($"Message content: {message.Value}");
        });
    }

    public ILibrary Library { get; } = new Library();
}
