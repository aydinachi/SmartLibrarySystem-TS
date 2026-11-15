
using SmartLibrarySystem.Modules;
using SmartLibrarySystem.Services;

namespace SmartLibrarySystem;

class Program
{
    static void Main(string[] args)
    {
        LibraryService library = new LibraryService();

        UserManagement userManagement = new UserManagement(library);
        BookManagement bookManagement = new BookManagement(library);
        LoanManagement loanManagement = new LoanManagement(library);

        while (true)
        {
            Console.Clear();

            Console.WriteLine("========================================");
            Console.WriteLine("       SMART LIBRARY MANAGEMENT");
            Console.WriteLine("========================================");
            Console.WriteLine("1. User Management");
            Console.WriteLine("2. Book Inventory");
            Console.WriteLine("3. Loan Management");
            Console.WriteLine("0. Exit");
            Console.WriteLine("========================================");

            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    userManagement.ShowMenu();
                    break;

                case "2":
                    bookManagement.ShowMenu();
                    break;

                case "3":
                    loanManagement.ShowMenu();
                    break;

                case "0":
                    Console.WriteLine("\nThank you for using Smart Library!");
                    return;

                default:
                    Console.WriteLine("\nInvalid option. Try again.");
                    Console.ReadKey();
                    break;
            }
        }
    }
}
