/*
Write a program that accepts a 9-element array representing a Sudoku row.
Validates if the row:
- Has all numbers from 1 to 9.
- Has no duplicates.
- Displays if the row is valid or invalid.
*/

namespace Tasks
{
    internal class Program_10
    {
        static bool ValidateArray(int[] arr)
        {
            bool[] numbers = new bool[10]; // Default value is false

            foreach (int num in arr)
            {
                if (num >= 1 && num <= 9)
                {
                    numbers[num] = true;
                }
                else
                {
                    return false;
                }
            }

            for (int index = 1; index <= 9; index++)
            {
                if (!numbers[index])
                {
                    return false;
                }
            }

            return true;
        }
        static void GetArrayElements()
        {
            int[] arr = new int[9];
            Console.WriteLine("Please enter the 9 elements in the row");

            for (int ctr = 1; ctr <= 9; ctr++)
            {
                Console.Write($"Element {ctr}: ");
                while (!int.TryParse(Console.ReadLine(), out arr[ctr - 1]))
                {
                    Console.WriteLine("Ivalid input. Try again.");
                }
            }

            if (ValidateArray(arr))
            {
                Console.WriteLine("The Row is valid.");
            }
            else
            {
                Console.WriteLine("The Row is invalid.");
            }
        }
        public void Run()
        {
            GetArrayElements();
        }
    }
}