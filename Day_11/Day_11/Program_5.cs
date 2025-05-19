
// Take 10 numbers from user and print the number of numbers that are divisible by 7	

namespace Tasks
{
    internal class Program_5
    {
        static bool IsDivisibleBy7(int number)
        {
            return number % 7 == 0;
        }

        static int CountDivisibleBy7()
        {
            int count = 0;
            Console.WriteLine("Enter 10 numbers:");

            for (int i = 1; i <= 10; i++)
            {
                Console.Write($"Enter Number {i}: ");
                int input;
                while (!int.TryParse(Console.ReadLine(), out input))
                {
                    Console.Write("Invalid input. Please enter a valid integer: ");
                }

                if (IsDivisibleBy7(input))
                {
                    count++;
                }
            }

            return count;
        }

        public void Run()
        {
            int result = CountDivisibleBy7();
            Console.WriteLine($"\nTotal numbers divisible by 7: {result}");
        }
    }

}
