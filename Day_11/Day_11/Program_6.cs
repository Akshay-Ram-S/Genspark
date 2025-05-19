/*
Count the Frequency of Each Element
Given an array, count the frequency of each element and print the result.
Input: {1, 2, 2, 3, 4, 4, 4}
*/

namespace Tasks
{
    internal class Program_6
    {
        static void PrintFrequency(int[] array)
        {
            Dictionary<int, int> frequency = new Dictionary<int, int>();

            foreach (int item in array)
            {
                if (frequency.ContainsKey(item))
                {
                    frequency[item]++;
                }
                else
                {
                    frequency[item] = 1;
                }
            }

            foreach (var pair in frequency)
            {
                Console.WriteLine($"{pair.Key} occurs {pair.Value} times");
            }
        }

        static void GetInputArray()
        {
            int n;
            Console.Write("Please enter the size of the array: ");
            while (!int.TryParse(Console.ReadLine(), out n))
            {
                Console.WriteLine("Invalid input. Try again");
            }

            if (n == 0)
                return;

            int[] arr = new int[n];
            Console.WriteLine("Please enter the array elements");

            for (int index = 0; index < n; index++)
            {
                while (!int.TryParse(Console.ReadLine(), out arr[index]))
                {
                    Console.WriteLine("Invalid input. Try again");
                }
            }
            PrintFrequency(arr);
        }

        public void Run()
        {
            GetInputArray();
        }
    }
}
