/*
Given two integer arrays, merge them into a single array.
Input: {1, 3, 5} and {2, 4, 6}
Output: {1, 3, 5, 2, 4, 6}
*/

namespace Tasks
{
    internal class Program_8
    {
        static int[] MergeArray(int[] arr1, int[] arr2)
        {
            int[] arr = new int[arr1.Length + arr2.Length];
            int index = 0;

            foreach (int num in arr1)
            {
                arr[index++] = num;
            }

            foreach (int num in arr2)
            {
                arr[index++] = num;
            }

            return arr;
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
            int[] arr1 = GetArray();
            int[] arr2 = GetArray();
            int[] arr3 = MergeArray(arr1, arr2);

            foreach (int num in arr3)
            {
                Console.Write(num + " ");
            }
        }
    }
}