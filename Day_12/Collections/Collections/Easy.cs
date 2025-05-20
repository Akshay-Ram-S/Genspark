using System;
using System.Collections.Generic;

namespace Easy
{
    internal class EmployeePromotion
    {
        private List<string> employeeList;

        public EmployeePromotion()
        {
            employeeList = new List<string>();
        }

        public void AddEmployee(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                employeeList.Add(name);
            }
        }

        public void FindEmployeePosition(string empName)
        {
            int index = employeeList.IndexOf(empName);
            if (index == -1)
                Console.WriteLine($"{empName} is not in promotion list");
            else
                Console.WriteLine($"\"{empName}\" is in the position {index + 1} for promotion.\n");
        }

        public void OptimizeListMemory()
        {
            Console.WriteLine($"The current size of the collection is: {employeeList.Capacity}");
            employeeList.TrimExcess();
            Console.WriteLine($"The size after removing the extra space is: {employeeList.Capacity}");
        }

        public void SortEmployees()
        {
            employeeList.Sort();
            foreach (string name in employeeList)
            {
                Console.WriteLine(name);
            }
        }


        public void Run()
        {
            Console.WriteLine("\nPlease enter the employee names in the order of their eligibility for promotion");
            Console.WriteLine("(Please enter blank to stop)");

            while (true)
            {
                string? input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                     break;

                AddEmployee(input);
            }

            while (true)
            {
                Console.WriteLine("\n1. Check promotion position");
                Console.WriteLine("2. Optimize list memory");
                Console.WriteLine("3. Sort and display list");
                Console.WriteLine("4. Exit");
                Console.Write("Enter your choice: ");

                int choice;
                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid input. Try again");
                }

                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Please enter the name of the employee to check promotion position");
                        string? empName = Console.ReadLine();
                        FindEmployeePosition(empName);
                        break;

                    case 2:
                        OptimizeListMemory();
                        break;

                    case 3:
                        SortEmployees();
                        break;

                    case 4:
                        return;

                    default:
                        Console.WriteLine("Invalid Choice. Try again");
                        break;
                }
            }

            

        }

    }

}

