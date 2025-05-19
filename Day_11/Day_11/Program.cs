namespace Tasks
{
    class Program
    {
        static void Main()
        {
            while (true) {
                Console.WriteLine("\nTask List:");
                Console.WriteLine("1  - Greet the user by name");
                Console.WriteLine("2  - Find the largest of two numbers");
                Console.WriteLine("3  - Perform an arithmetic operation on two numbers");
                Console.WriteLine("4  - Login system with 3 attempts");
                Console.WriteLine("5  - Count numbers divisible by 7");
                Console.WriteLine("6  - Frequency count of elements in an array");
                Console.WriteLine("7  - Left rotate array by one position");
                Console.WriteLine("8  - Merge two integer arrays");
                Console.WriteLine("9  - Word guess game (4-letter word)");
                Console.WriteLine("10 - Validate a Sudoku row");
                Console.WriteLine("11 - Validate entire Sudoku board");
                Console.WriteLine("12 - Encrypt/Decrypt message (shift by 3)");
                Console.WriteLine("0  - Exit");
                Console.WriteLine("\nEnter the task number: ");
                int task = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine();
                switch (task)
                {
                    case 0:
                        Console.WriteLine("Exit Application");
                        return;

                    case 1:
                        var t1 = new Program_1();
                        t1.Run();
                        break;

                    case 2:
                        var t2 = new Program_2();
                        t2.Run();
                        break;

                    case 3:
                        var t3 = new Program_3();
                        t3.Run();
                        break;

                    case 4:
                        var t4 = new Program_4();
                        t4.Run();
                        break;

                    case 5:
                        var t5 = new Program_5();
                        t5.Run();
                        break;

                    case 6:
                        var t6 = new Program_6();
                        t6.Run();
                        break;

                    case 7:
                        var t7 = new Program_7();
                        t7.Run();
                        break;

                    case 8:
                        var t8 = new Program_8();
                        t8.Run();
                        break;

                    case 9:
                        var t9 = new Program_9();
                        t9.Run();
                        break;

                    case 10:
                        var t10 = new Program_10();
                        t10.Run();
                        break;

                    case 11:
                        var t11 = new Program_11();
                        t11.Run();
                        break;

                    case 12:
                        var t12 = new Program_12();
                        t12.Run();
                        break;

                    default:
                        Console.WriteLine("Invalid choice");
                        break;
                }
                Console.WriteLine("\nDo you want to continue (y/n)");
                char ch = Console.ReadKey().KeyChar;
                if (ch == 'n')
                    break;
            }
            
        }
    }
}
