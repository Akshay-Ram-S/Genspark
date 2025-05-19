/*
Create a program to rotate the array to the left by one position.
Input: {10, 20, 30, 40, 50}
Output: {20, 30, 40, 50, 10}
*/

namespace Tasks
{
    internal class Program_7
    {
        static void RotateLeftByOne(int[] arr)
        {
            int firstElement = arr[0];
            for (int i = 0; i < arr.Length - 1; i++)
            {
                arr[i] = arr[i + 1];
            }
            arr[arr.Length - 1] = firstElement;
        }

        static void PrintArray(int[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Console.Write(arr[i] + " ");
            }
        }
        static int[] GetArray()
        {
            int n;
            Console.Write("Please enter the size of the array: ");
            while (!int.TryParse(Console.ReadLine(), out n))
            {
                Console.WriteLine("Invalid input. Try again");
            }

            int[] arr = new int[n];
            Console.WriteLine("Please enter the array elements");

            for (int index = 0; index < n; index++)
            {
                while (!int.TryParse(Console.ReadLine(), out arr[index]))
                {
                    Console.WriteLine("Invalid input. Try again");
                }
            }

            return arr;
        }

        public void Run()
        {
            int[] array = GetArray();

            
            Console.WriteLine("Original array:");
            PrintArray(array);

            if (array.Length > 1)
            {
                RotateLeftByOne(array);
            }

            Console.WriteLine("\nArray after left rotation by one position:");
            PrintArray(array);
        }
    }
}