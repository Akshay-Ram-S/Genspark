//Take 2 numbers from user and print the largest

namespace Tasks
{
    internal class Program_2
    {
        static int GetLargest(int num1, int num2)
        {
            return (num1 > num2) ? num1 : num2;
        }

        static void getInputs()
        {
            int num1, num2;
            Console.Write("Please enter the first number: ");
            while (!int.TryParse(Console.ReadLine(), out num1))
            {
                Console.WriteLine("Invalid input, try again");
            }

            Console.Write("Please enter the second number: ");
            while (!int.TryParse(Console.ReadLine(), out num2))
            {
                Console.WriteLine("Invalid input, try again");
            }

            int largest = GetLargest(num1, num2);
            Console.WriteLine("The largest number is: " + largest);
        }
        public void Run()
        {
            getInputs();
        }
    }
}

