/*
Write a program that:

Takes a message string as input (only lowercase letters, no spaces or symbols).
Encrypts it by shifting each character forward by 3 places in the alphabet.
Decrypts it back to the original message by shifting backward by 3.
Handles wrap-around, e.g., 'z' becomes 'c'.
*/

namespace Tasks
{
    using System.Text.RegularExpressions;

    internal class Program_12
    {
        static string Encrypt(string msg, int shift)
        {
            string? encryptedMsg = "";
            foreach (char ch in msg)
            {
                char shifted = (char)(((ch - 'a' + shift) % 26) + 'a');
                encryptedMsg += shifted;
            }

            return encryptedMsg;
        }

        static string Decrypt(string msg, int shift)
        {
            string? decryptedMsg = "";
            foreach (char ch in msg)
            {
                char shifted = (char)(((26 + ch - 'a' - shift) % 26) + 'a');
                decryptedMsg += shifted;
            }

            return decryptedMsg;
        }

        static bool ValidateInputMessage(string msg)
        {
            if (!Regex.IsMatch(msg, @"^[a-z]+$"))
            {
                Console.WriteLine("Invalid input. Please enter only lowercase letters with no spaces or symbols.");
                return false;
            }
            return true;
        }
        static void GetInput()
        {
            string? message;
            Console.WriteLine("Please enter the message to encrypt: ");
            while (true)
            {
                message = Console.ReadLine();
                if (ValidateInputMessage(message))
                    break;
            }

            int shift;
            Console.WriteLine("Enter the no. of shifts: ");
            while (!int.TryParse(Console.ReadLine(), out shift))
            {
                Console.WriteLine("Invalid input, try again");
            }
            

            string encryptedMsg = Encrypt(message, shift);
            Console.WriteLine($"Encrypted Message: {encryptedMsg}");

            string decryptedMsg = Decrypt(encryptedMsg, shift);
            Console.WriteLine($"Decrypted Message: {decryptedMsg}");
        }
        public void Run()
        {
            GetInput();
        }
    }
}

