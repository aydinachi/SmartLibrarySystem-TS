
using SmartLibrarySystem.Models;
using SmartLibrarySystem.Services;
using SmartLibrarySystem.Utilities;

namespace SmartLibrarySystem.Modules;

public class UserManagement
{
    private readonly LibraryService _library;

    public UserManagement(LibraryService library)
    {
        _library = library;
    }

    public void ShowMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("========== USER MANAGEMENT ==========");
            Console.WriteLine("1. Register User");
            Console.WriteLine("2. Update User");
            Console.WriteLine("3. Delete User");
            Console.WriteLine("4. View All Users");
            Console.WriteLine("5. Search User");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nSelect option: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterUser();
                    break;
                case "2":
                    UpdateUser();
                    break;
                case "3":
                    DeleteUser();
                    break;
                case "4":
                    DisplayUsers();
                    break;
                case "5":
                    SearchUser();
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

    private void RegisterUser()
    {
        Console.Clear();
        Console.WriteLine("========== REGISTER USER ==========");

        string name = InputValidator.ReadRequiredString("Full name: ");
        string email = InputValidator.ReadEmail("Email: ");

        if (_library.Users.Any(u =>
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("A user with this email already exists.");
            InputValidator.Pause();
            return;
        }

        Console.WriteLine("\nSelect role:");
        Console.WriteLine("1. Administrator");
        Console.WriteLine("2. Member");

        int roleChoice = InputValidator.ReadInt("Role: ");

        if (!Enum.IsDefined(typeof(UserRole), roleChoice))
        {
            Console.WriteLine("Invalid role.");
            InputValidator.Pause();
            return;
        }

        UserRole role = (UserRole)roleChoice;

        User user = new User(
            _library.GetNextUserId(),
            name,
            email,
            role
        );

        _library.Users.Add(user);

        Console.WriteLine("\nUser registered successfully!");
        InputValidator.Pause();
    }

    private void UpdateUser()
    {
        Console.Clear();
        DisplayUsersWithoutPause();

        if (_library.Users.Count == 0)
        {
            InputValidator.Pause();
            return;
        }

        int id = InputValidator.ReadInt("\nEnter user ID to update: ");

        User? user = _library.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            InputValidator.Pause();
            return;
        }

        Console.WriteLine("\nLeave a field empty to keep its current value.");

        Console.Write($"Full name ({user.FullName}): ");
        string? name = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(name))
            user.FullName = name.Trim();

        Console.Write($"Email ({user.Email}): ");
        string? email = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(email))
        {
            email = email.Trim();

            bool validEmail = false;

            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                validEmail = address.Address == email &&
                             email.Contains('.') &&
                             !email.StartsWith("@");
            }
            catch (FormatException)
            {
                validEmail = false;
            }

            bool duplicate = _library.Users.Any(u =>
                u.Id != id &&
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

            if (!validEmail || duplicate)
            {
                Console.WriteLine("Invalid or already registered email. Email was not changed.");
            }
            else
            {
                user.Email = email;
            }
        }

        Console.WriteLine("\nSelect new role:");
        Console.WriteLine("1. Administrator");
        Console.WriteLine("2. Member");
        Console.WriteLine("0. Keep current role");

        int role = InputValidator.ReadInt("Role: ", 0);

        if (role == 1 || role == 2)
            user.Role = (UserRole)role;
        else if (role != 0)
            Console.WriteLine("Invalid role. Current role was kept.");

        Console.WriteLine("\nUser updated successfully!");
        InputValidator.Pause();
    }

    private void DeleteUser()
    {
        Console.Clear();
        DisplayUsersWithoutPause();

        if (_library.Users.Count == 0)
        {
            InputValidator.Pause();
            return;
        }

        int id = InputValidator.ReadInt("\nEnter user ID to delete: ");

        User? user = _library.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            Console.WriteLine("User not found.");
            InputValidator.Pause();
            return;
        }

        bool hasActiveLoan = _library.Loans.Any(l =>
            l.UserId == id && !l.IsReturned);

        if (hasActiveLoan)
        {
            Console.WriteLine("Cannot delete a user with active loans.");
            InputValidator.Pause();
            return;
        }

        Console.Write($"Are you sure you want to delete {user.FullName}? (y/n): ");
        string? confirmation = Console.ReadLine();

        if (confirmation?.Trim().ToLower() == "y")
        {
            _library.Users.Remove(user);
            Console.WriteLine("User deleted successfully.");
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }

        InputValidator.Pause();
    }

    private void DisplayUsersWithoutPause()
    {
        Console.WriteLine("========== REGISTERED USERS ==========");

        if (_library.Users.Count == 0)
        {
            Console.WriteLine("No registered users.");
            return;
        }

        foreach (User user in _library.Users)
        {
            Console.WriteLine(
                $"ID: {user.Id} | Name: {user.FullName} | " +
                $"Email: {user.Email} | Role: {user.Role}"
            );
        }
    }

    private void DisplayUsers()
    {
        Console.Clear();
        DisplayUsersWithoutPause();
        InputValidator.Pause();
    }

    private void SearchUser()
    {
        Console.Clear();

        int id = InputValidator.ReadInt("Enter user ID: ");

        User? user = _library.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            Console.WriteLine("User not found.");
        }
        else
        {
            Console.WriteLine($"\nID: {user.Id}");
            Console.WriteLine($"Name: {user.FullName}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Role: {user.Role}");
        }

        InputValidator.Pause();
    }
}
