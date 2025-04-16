# Library Manager (MAUI)

Library Manager is a cross-platform application developed in **C#** using the **.NET MAUI** framework. This project is a migration of the original **WPF-based Library Manager** ([BookLibraryManager](https://github.com/ryik5/BookLibraryManager)) to MAUI, allowing it to run on multiple platforms, including **Windows, MacOS, Android, and iOS**.

## Features

- **Book Management**: Add, edit, and remove books from the library.
- **Categories & Authors**: Organize books by categories and authors (Planned).
- **Search & Filter**: Easily find books using various search and filter options.
- **Multi-Platform Support**: Runs on Windows(WPF), MacOSX, Android (Planned), and iOS (Planned).
- **Modern UI**: Built using **.NET MAUI** for a responsive and adaptive user interface.
- **Local Storage**: Saves book data locally using SQLite (Planned). Now it uses XML.
- **Cross-Platform Synchronization** (Planned): Future versions may include cloud sync support.

## Installation

### Prerequisites

- Install [.NET SDK 8.0 or later](https://dotnet.microsoft.com/download/dotnet/8.0)
- Install **Visual Studio 2022**(for Windows) with **.NET MAUI workload**
- Ensure the target platform SDKs (Android, iOS, MacOSX) and dependencies (Xcode, Xcode CLI, .NET, .NET CLI) are installed if developing for those platforms

### Clone the Repository

```sh
git clone https://github.com/ryik5/LibraryManager.git
cd LibraryManager
```

### Build and Run

#### Windows

```sh
dotnet build
```

#### MacOSX

```sh
dotnet restore

dotnet build
```

Run the project in Visual Studio by selecting the target platform (Windows/macOS).

#### Android/iOS

For Android:

```sh
dotnet build -t:Run -f net8.0-android
```

For iOS (requires macOS and Xcode):

```sh
dotnet build -t:Run -f net8.0-ios
```

## Usage

1. **Launch the Application**: Open Library Manager on your preferred platform.
2. **Create/Load a Library**: Click on the "Create Library" or "Load Library" and pick up the previously saved library
3. **Add a New Book**: Click on the "Add Book" button, enter details, and save.
4. **Edit/Delete Books**: Select a book from the list to modify or remove it.
5. **Filter/Search**: Use the search bar or category filters to find books quickly.
6. **Manage Authors & Categories**: Organize books efficiently by assigning authors and categories. (Planned)

## Screenshots

![Library](/docs/Library.png)

*The main dashboard displaying an overview of your collection.*

![Books View](/docs/Books.png)

*The 'Books view' interface.*

![Edit Book](/docs/EditBook.png)

*The 'Edit Book' interface with ISBN scanning feature.*

![Search Management](/docs/FindBooks.png)

*Find books in your library and check them.*


## Roadmap

- [ ] Implement cloud sync for book storage
- [ ] Improve UI for better cross-platform experience
- [ ] Add export/import functionality for book data
- [ ] Introduce user authentication and multi-user support

## Contribution

Contributions are welcome! Feel free to submit issues or pull requests.

## Contact

For questions or support, please contact **me** on **[github](https://github.com/ryik5)**.
