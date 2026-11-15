
using SmartLibrarySystem.Models;
using SmartLibrarySystem.Services;
using SmartLibrarySystem.Utilities;

namespace SmartLibrarySystem.Modules;

public class LoanManagement
{
    private readonly LibraryService _library;

    private const int LoanDurationDays = 14;
    private const decimal FinePerDay = 1.00m;

    public LoanManagement(LibraryService library)
    {
        _library = library;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== LOAN MANAGEMENT ==========");
            Console.WriteLine("1. Borrow Book");
            Console.WriteLine("2. Return Book");
            Console.WriteLine("3. View Active Loans");
            Console.WriteLine("4. View Overdue Loans");
            Console.WriteLine("5. Borrowing History");
            Console.WriteLine("6. Calculate Fine");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    BorrowBook();
                    break;
                case "2":
                    ReturnBook();
                    break;
                case "3":
                    DisplayActiveLoans();
                    break;
                case "4":
                    DisplayOverdueLoans();
                    break;
                case "5":
                    DisplayHistory();
                    break;
                case "6":
                    DisplayFine();
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

    private void BorrowBook()
    {
        Console.Clear();
        Console.WriteLine("========== BORROW BOOK ==========");

        if (_library.Users.All(u => u.Role != UserRole.Member))
        {
            Console.WriteLine("No registered members available.");
            InputValidator.Pause();
            return;
        }

        int userId = InputValidator.ReadInt("Enter member ID: ");

        User? user = _library.Users.FirstOrDefault(u =>
            u.Id == userId && u.Role == UserRole.Member);

        if (user == null)
        {
            Console.WriteLine("Member not found.");
            InputValidator.Pause();
            return;
        }

        int bookId = InputValidator.ReadInt("Enter book ID: ");

        Book? book = _library.Books.FirstOrDefault(b => b.Id == bookId);

        if (book == null)
        {
            Console.WriteLine("Book not found.");
            InputValidator.Pause();
            return;
        }

        if (!book.IsAvailable)
        {
            Console.WriteLine("This book is currently unavailable.");
            InputValidator.Pause();
            return;
        }

        Loan loan = new Loan(
            _library.GetNextLoanId(),
            userId,
            bookId
        );

        _library.Loans.Add(loan);
        book.IsAvailable = false;

        Console.WriteLine("\nBook borrowed successfully!");
        Console.WriteLine($"Member: {user.FullName}");
        Console.WriteLine($"Book: {book.Title}");
        Console.WriteLine($"Borrow date: {loan.BorrowDate:dd.MM.yyyy}");
        Console.WriteLine($"Due date: {loan.DueDate:dd.MM.yyyy}");

        InputValidator.Pause();
    }

    private void ReturnBook()
    {
        Console.Clear();
        Console.WriteLine("========== RETURN BOOK ==========");

        List<Loan> activeLoans = _library.Loans
            .Where(l => !l.IsReturned)
            .ToList();

        if (activeLoans.Count == 0)
        {
            Console.WriteLine("There are no active loans.");
            InputValidator.Pause();
            return;
        }

        foreach (Loan loan in activeLoans)
        {
            User? user = _library.Users.FirstOrDefault(u => u.Id == loan.UserId);
            Book? book = _library.Books.FirstOrDefault(b => b.Id == loan.BookId);

            Console.WriteLine(
                $"Loan ID: {loan.Id} | " +
                $"Member: {user?.FullName ?? "Unknown"} | " +
                $"Book: {book?.Title ?? "Unknown"} | " +
                $"Due: {loan.DueDate:dd.MM.yyyy}"
            );
        }

        int loanId = InputValidator.ReadInt("\nEnter loan ID: ");

        Loan? selectedLoan = activeLoans.FirstOrDefault(l => l.Id == loanId);

        if (selectedLoan == null)
        {
            Console.WriteLine("Active loan not found.");
            InputValidator.Pause();
            return;
        }

        DateTime returnDate = DateTime.Today;

        selectedLoan.ReturnDate = returnDate;
        selectedLoan.Fine = CalculateFine(selectedLoan, returnDate);

        Book? returnedBook = _library.Books.FirstOrDefault(
            b => b.Id == selectedLoan.BookId
        );

        if (returnedBook != null)
            returnedBook.IsAvailable = true;

        Console.WriteLine("\nBook returned successfully!");
        Console.WriteLine($"Return date: {returnDate:dd.MM.yyyy}");
        Console.WriteLine($"Late fee: {selectedLoan.Fine:F2} KM");

        InputValidator.Pause();
    }

    public decimal CalculateFine(Loan loan, DateTime returnDate)
    {
        if (returnDate.Date <= loan.DueDate.Date)
            return 0;

        int lateDays = (returnDate.Date - loan.DueDate.Date).Days;

        return lateDays * FinePerDay;
    }

    private void DisplayActiveLoans()
    {
        Console.Clear();
        Console.WriteLine("========== ACTIVE LOANS ==========");

        List<Loan> activeLoans = _library.Loans
            .Where(l => !l.IsReturned)
            .ToList();

        if (activeLoans.Count == 0)
        {
            Console.WriteLine("No active loans.");
        }

        foreach (Loan loan in activeLoans)
        {
            User? user = _library.Users.FirstOrDefault(u => u.Id == loan.UserId);
            Book? book = _library.Books.FirstOrDefault(b => b.Id == loan.BookId);

            Console.WriteLine(
                $"Loan ID: {loan.Id} | " +
                $"Member: {user?.FullName ?? "Unknown"} | " +
                $"Book: {book?.Title ?? "Unknown"} | " +
                $"Borrowed: {loan.BorrowDate:dd.MM.yyyy} | " +
                $"Due: {loan.DueDate:dd.MM.yyyy}"
            );
        }

        InputValidator.Pause();
    }

    private void DisplayOverdueLoans()
    {
        Console.Clear();
        Console.WriteLine("========== OVERDUE LOANS ==========");

        List<Loan> overdueLoans = _library.Loans
            .Where(l => !l.IsReturned && DateTime.Today > l.DueDate.Date)
            .ToList();

        if (overdueLoans.Count == 0)
        {
            Console.WriteLine("No overdue loans.");
        }

        foreach (Loan loan in overdueLoans)
        {
            User? user = _library.Users.FirstOrDefault(u => u.Id == loan.UserId);
            Book? book = _library.Books.FirstOrDefault(b => b.Id == loan.BookId);

            decimal fine = CalculateFine(loan, DateTime.Today);

            Console.WriteLine(
                $"Loan ID: {loan.Id} | " +
                $"Member: {user?.FullName ?? "Unknown"} | " +
                $"Book: {book?.Title ?? "Unknown"} | " +
                $"Due: {loan.DueDate:dd.MM.yyyy} | " +
                $"Current fine: {fine:F2} KM"
            );
        }

        InputValidator.Pause();
    }

    private void DisplayHistory()
    {
        Console.Clear();
        Console.WriteLine("========== BORROWING HISTORY ==========");

        if (_library.Loans.Count == 0)
        {
            Console.WriteLine("No borrowing history.");
            InputValidator.Pause();
            return;
        }

        foreach (Loan loan in _library.Loans.OrderByDescending(l => l.BorrowDate))
        {
            User? user = _library.Users.FirstOrDefault(u => u.Id == loan.UserId);
            Book? book = _library.Books.FirstOrDefault(b => b.Id == loan.BookId);

            Console.WriteLine(
                $"Loan ID: {loan.Id} | " +
                $"Member: {user?.FullName ?? "Unknown"} | " +
                $"Book: {book?.Title ?? "Unknown"}"
            );

            Console.WriteLine(
                $"Borrowed: {loan.BorrowDate:dd.MM.yyyy} | " +
                $"Due: {loan.DueDate:dd.MM.yyyy} | " +
                $"Returned: {(loan.ReturnDate.HasValue ? loan.ReturnDate.Value.ToString("dd.MM.yyyy") : "Not returned")} | " +
                $"Fine: {loan.Fine:F2} KM"
            );

            Console.WriteLine("---------------------------------------");
        }

        InputValidator.Pause();
    }

    private void DisplayFine()
    {
        Console.Clear();
        Console.WriteLine("========== FINE CALCULATOR ==========");

        int loanId = InputValidator.ReadInt("Enter loan ID: ");

        Loan? loan = _library.Loans.FirstOrDefault(l => l.Id == loanId);

        if (loan == null)
        {
            Console.WriteLine("Loan not found.");
        }
        else
        {
            DateTime calculationDate = loan.ReturnDate ?? DateTime.Today;

            decimal fine = CalculateFine(loan, calculationDate);

            Console.WriteLine($"Due date: {loan.DueDate:dd.MM.yyyy}");
            Console.WriteLine($"Calculation date: {calculationDate:dd.MM.yyyy}");
            Console.WriteLine($"Fine: {fine:F2} KM");
        }

        InputValidator.Pause();
    }
}
