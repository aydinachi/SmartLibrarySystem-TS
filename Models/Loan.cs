
namespace SmartLibrarySystem.Models;

public class Loan
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int BookId { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }

    public decimal Fine { get; set; }

    public bool IsReturned => ReturnDate.HasValue;

    public Loan(int id, int userId, int bookId)
    {
        Id = id;
        UserId = userId;
        BookId = bookId;

        BorrowDate = DateTime.Today;
        DueDate = BorrowDate.AddDays(14);
        ReturnDate = null;
        Fine = 0;
    }
}
