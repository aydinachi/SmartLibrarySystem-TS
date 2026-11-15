
using System.Net.Mail;

namespace SmartLibrarySystem.Utilities;

public static class InputValidator
{
    public static string ReadRequiredString(string message)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
                return input.Trim();

            Console.WriteLine("Input cannot be empty. Try again.");
        }
    }

    public static int ReadInt(string message, int min = 1)
    {
        while (true)
        {
            Console.Write(message);
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int value) && value >= min)
                return value;

            Console.WriteLine($"Please enter a valid number (minimum {min}).");
        }
    }

    public static string ReadEmail(string message)
    {
        while (true)
        {
            string email = ReadRequiredString(message);

            try
            {
                MailAddress address = new MailAddress(email);

                if (address.Address == email &&
                    email.Contains('.') &&
                    !email.StartsWith("@"))
                {
                    return email;
                }
            }
            catch (FormatException)
            {
                // Invalid email format.
            }

            Console.WriteLine("Invalid email address. Try again.");
        }
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
