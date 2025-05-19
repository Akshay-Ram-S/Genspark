// Take 2 numbers from user, check the operation user wants to perform (+,-,*,/). Do the operation and print the result.

namespace Tasks
{
    internal class Program_3
    {
        static void Calculate(double num1, double num2, char operation)
        {
            double result = 0;
            bool validOperation = true;  // Flag to check if the operation is valid

            switch (operation)
            {
                case '+':
                    result = num1 + num2;
                    break;
                case '-':
                    result = num1 - num2;
                    break;
                case '*':
                    result = num1 * num2;
                    break;
                case '/':
                    if (num2 != 0)
                    {
                        result = num1 / num2;
                    }
                    else
                    {
                        Console.WriteLine("Error: Division by zero.");
                        validOperation = false;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid operation.");
                    validOperation = false;
                    break;
            }

            if (validOperation)
            {
                Console.WriteLine($"Result = {result}");
            }
        }

        public void Run()
        {
            Console.Write("Enter the first number: ");
            double number1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter the second number: ");
            double number2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter the operation (+, -, *, /): ");
            char operation = Convert.ToChar(Console.ReadLine());

            Calculate(number1, number2, operation);
        }
    }
}
