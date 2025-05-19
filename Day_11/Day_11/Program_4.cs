/*
Take username and password from user. Check if user name is "Admin" and password is "pass" if yes then print success message.
Give 3 attempts to user. In the end of eh 3rd attempt if user still is unable to provide valid creds then exit the application after print the message 
"Invalid attempts for 3 times. Exiting...." 
*/

namespace Tasks
{
    internal class Program_4
    {
        static void Login()
        {
            int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                Console.Write("Enter username: ");
                string? username = Console.ReadLine();

                Console.Write("Enter password: ");
                string? password = Console.ReadLine();

                if (username == "Admin" && password == "pass")
                {
                    Console.WriteLine("Login successful! Welcome, Admin.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Invalid Attempt {attempt} of {maxAttempts}.");
                }
            }

            Console.WriteLine("Invalid attempts for 3 times. Exiting....");
        }

        public void Run()
        {
            Login();
        }
    }
}

