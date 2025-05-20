using System;
using System.Collections.Generic;
using System.Linq;
using employee;

namespace Medium
{
    internal class EmployeeAggregate
    {
        Dictionary<int, Employee> employeeDirectory = new Dictionary<int, Employee>();
        List<Employee> employeeList = new List<Employee>();

        public string GetNonEmptyString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty. Please try again.");
                }
            } while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public void AddEmployee()
        {
            int id, age;
            string name;
            double salary;

            Console.Write("Please enter the employee ID: ");
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || employeeDirectory.ContainsKey(id))
            {
                if (employeeDirectory.ContainsKey(id))
                    Console.WriteLine("An employee with this ID already exists.");
                else
                    Console.WriteLine("Invalid input. Try again.");
            }

            name = GetNonEmptyString("Please enter the employee name: ");

            Console.Write("Please enter the employee age: ");
            while (!int.TryParse(Console.ReadLine(), out age))
            {
                Console.WriteLine("Invalid input. Try again.");
            }

            Console.Write("Please enter the employee salary: ");
            while (!double.TryParse(Console.ReadLine(), out salary))
            {
                Console.WriteLine("Invalid input. Try again.");
            }

            Employee emp = new Employee(id, age, name, salary);
            employeeDirectory[id] = emp;
            employeeList.Add(emp);

            Console.WriteLine("Employee added successfully.\n");
        }

        public void ShowSortedEmployees()
        {
            employeeList.Sort();
            Console.WriteLine("\nEmployees sorted by salary:");
            Console.WriteLine("--------------------");
            foreach (var emp in employeeList)
            {
                Console.WriteLine(emp);
                Console.WriteLine("--------------------");
            }

        }

        public void SearchByName()
        {
            string name = GetNonEmptyString("Enter the employee name to search: ");
            var employeesWithName = employeeList.Where(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

            if (employeesWithName.Any())
            {
                Console.WriteLine($"\nEmployees with name {name}:");
                foreach (var emp in employeesWithName)
                {
                    Console.WriteLine(emp);
                }
            }
            else
            {
                Console.WriteLine($"No employees found with the name {name}.");
            }
        }

        public void SearchById()
        {
            int id;
            Console.Write("Enter Employee ID to search: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Enter a valid integer.");
            }

            var emp = employeeList.FirstOrDefault(e => e.Id == id);
            if (emp != null)
            {
                Console.WriteLine($"\nEmployee Found:\n{emp}");
            }
            else
            {
                Console.WriteLine($"Employee with ID {id} not found.");
            }
        }

        public void ShowOlderEmployees()
        {
            int id;
            Console.Write("Enter Employee ID: ");
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.WriteLine("Invalid input. Try again.");
            }

            var emp = employeeList.FirstOrDefault(e => e.Id == id);

            if (emp != null)
            {
                var olderEmployees = employeeList.Where(e => e.Age > emp.Age).ToList();

                if (olderEmployees.Any())
                {
                    Console.WriteLine($"\nEmployees older than {emp.Name} ({emp.Age} years old):");
                    foreach (var olderEmp in olderEmployees)
                    {
                        Console.WriteLine(olderEmp);
                    }
                }
                else
                {
                    Console.WriteLine($"No employees are older than {emp.Name}.");
                }
            }
            else
            {
                Console.WriteLine($"Employee with ID {id} not found.");
            }

        }

        public void ShowMenu()
        {
            Console.WriteLine("\n--------- MENU -----------");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. View Employees Sorted by Salary");
            Console.WriteLine("3. Search Employees by Name");
            Console.WriteLine("4. Search Employee by ID");
            Console.WriteLine("5. Find Older Employees");
            Console.WriteLine("6. Exit");
            Console.WriteLine("-----------------------------");
        }

        public void Run()
        {
            int choice;
            do
            {
                ShowMenu();
                Console.Write("Enter your choice (1-6): ");
                while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > 6)
                {
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 6.");
                }

                switch (choice)
                {
                    case 1: AddEmployee(); break;
                    case 2: ShowSortedEmployees(); break;
                    case 3: SearchByName(); break;
                    case 4: SearchById(); break;
                    case 5: ShowOlderEmployees(); break;
                    case 6: Console.WriteLine("Exiting program. Goodbye!"); break;
                }

            } while (choice != 6);
        }
    }
}
