
using SmartLibrarySystem.Models;

namespace SmartLibrarySystem.Services;

public class LibraryService
{
    public List<User> Users { get; } = new();
    public List<Book> Books { get; } = new();
    public List<Loan> Loans { get; } = new();

    public int GetNextUserId()
    {
        return Users.Count == 0 ? 1 : Users.Max(u => u.Id) + 1;
    }

    public int GetNextBookId()
    {
        return Books.Count == 0 ? 1 : Books.Max(b => b.Id) + 1;
    }

    public int GetNextLoanId()
    {
        return Loans.Count == 0 ? 1 : Loans.Max(l => l.Id) + 1;
    }
}
