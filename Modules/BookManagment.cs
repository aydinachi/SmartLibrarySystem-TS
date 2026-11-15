
using SmartLibrarySystem.Models;
using SmartLibrarySystem.Services;
using SmartLibrarySystem.Utilities;

namespace SmartLibrarySystem.Modules;

public class BookManagement
{
    private readonly LibraryService _library;

    public BookManagement(LibraryService library)
    {
        _library = library;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== BOOK INVENTORY ==========");
            Console.WriteLine("1. Add Book");
            Console.WriteLine("2. Update Book");
            Console.WriteLine("3. Delete Book");
            Console.WriteLine("4. View All Books");
            Console.WriteLine("5. Search by Title");
            Console.WriteLine("6. Search by Author");
            Console.WriteLine("7. Search by Genre");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddBook();
                    break;
                case "2":
                    UpdateBook();
                    break;
                case "3":
                    DeleteBook();
                    break;
                case "4":
                    DisplayBooks();
                    break;
                case "5":
                    SearchBooks("title");
                    break;
                case "6":
                    SearchBooks("author");
                    break;
                case "7":
                    SearchBooks("genre");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    InputValidator.Pause();
                    break;
            }
        }
    }

    private void AddBook()
    {
        Console.Clear();
        Console.WriteLine("========== ADD BOOK ==========");

        string title = InputValidator.ReadRequiredString("Title: ");
        string author = InputValidator.ReadRequiredString("Author: ");
        string genre = InputValidator.ReadRequiredString("Genre: ");

        Book book = new Book(
            _library.GetNextBookId(),
            title,
            author,
            genre
        );

        _library.Books.Add(book);

        Console.WriteLine("\nBook added successfully!");
        InputValidator.Pause();
    }

    private void UpdateBook()
    {
        Console.Clear();
        DisplayBooksWithoutPause();

        if (_library.Books.Count == 0)
        {
            InputValidator.Pause();
            return;
        }

        int id = InputValidator.ReadInt("\nEnter book ID to update: ");

        Book? book = _library.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            InputValidator.Pause();
            return;
        }

        Console.WriteLine("\nLeave a field empty to keep its current value.");

        Console.Write($"Title ({book.Title}): ");
        string? title = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(title))
            book.Title = title.Trim();

        Console.Write($"Author ({book.Author}): ");
        string? author = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(author))
            book.Author = author.Trim();

        Console.Write($"Genre ({book.Genre}): ");
        string? genre = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(genre))
            book.Genre = genre.Trim();

        Console.WriteLine("\nBook updated successfully!");
        InputValidator.Pause();
    }

    private void DeleteBook()
    {
        Console.Clear();
        DisplayBooksWithoutPause();

        if (_library.Books.Count == 0)
        {
            InputValidator.Pause();
            return;
        }

        int id = InputValidator.ReadInt("\nEnter book ID to delete: ");

        Book? book = _library.Books.FirstOrDefault(b => b.Id == id);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            InputValidator.Pause();
            return;
        }

        bool hasActiveLoan = _library.Loans.Any(l =>
            l.BookId == id && !l.IsReturned);

        if (hasActiveLoan)
        {
            Console.WriteLine("Cannot delete a borrowed book.");
            InputValidator.Pause();
            return;
        }

        Console.Write($"Delete '{book.Title}'? (y/n): ");
        string? confirmation = Console.ReadLine();

        if (confirmation?.Trim().ToLower() == "y")
        {
            _library.Books.Remove(book);
            Console.WriteLine("Book deleted successfully.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }

        InputValidator.Pause();
    }

    private void DisplayBooksWithoutPause()
    {
        Console.WriteLine("========== BOOK INVENTORY ==========");

        if (_library.Books.Count == 0)
        {
            Console.WriteLine("No books available.");
            return;
        }

        foreach (Book book in _library.Books)
        {
            string availability = book.IsAvailable ? "Available" : "Borrowed";

            Console.WriteLine(
                $"ID: {book.Id} | Title: {book.Title} | " +
                $"Author: {book.Author} | Genre: {book.Genre} | " +
                $"Status: {availability}"
            );
        }
    }

    private void DisplayBooks()
    {
        Console.Clear();
        DisplayBooksWithoutPause();
        InputValidator.Pause();
    }

    private void SearchBooks(string searchType)
    {
        Console.Clear();

        string searchTerm = InputValidator.ReadRequiredString(
            $"Enter {searchType}: "
        );

        IEnumerable<Book> results;

        if (searchType == "title")
        {
            results = _library.Books.Where(b =>
                b.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        else if (searchType == "author")
        {
            results = _library.Books.Where(b =>
                b.Author.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            results = _library.Books.Where(b =>
                b.Genre.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        List<Book> foundBooks = results.ToList();

        Console.WriteLine("\n========== SEARCH RESULTS ==========");

        if (foundBooks.Count == 0)
        {
            Console.WriteLine("No matching books found.");
        }
        else
        {
            foreach (Book book in foundBooks)
            {
                Console.WriteLine(
                    $"ID: {book.Id} | {book.Title} | {book.Author} | " +
                    $"{book.Genre} | " +
                    $"{(book.IsAvailable ? "Available" : "Borrowed")}"
                );
            }
        }

        InputValidator.Pause();
    }
}
