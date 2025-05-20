using System;
using System.Collections.Generic;
using employee;

namespace Hard
{
    class EmployeeApp
    {
        Dictionary<int, Employee> employeeDirectory = new Dictionary<int, Employee>();

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

            Console.Write("Enter new employee ID: ");
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0 || employeeDirectory.ContainsKey(id))
            {
                if (employeeDirectory.ContainsKey(id))
                    Console.WriteLine("Employee with this ID already exists. Try a different ID:");
                else
                    Console.WriteLine("Invalid input. Please enter a valid positive integer for ID:");
            }

            name = GetNonEmptyString("Enter name: ");

            Console.Write("Enter age: ");
            while (!int.TryParse(Console.ReadLine(), out age) || age <= 0)
            {
                Console.WriteLine("Invalid input. Enter a positive integer for age:");
            }

            Console.Write("Enter salary: ");
            while (!double.TryParse(Console.ReadLine(), out salary) || salary < 0)
            {
                Console.WriteLine("Invalid input. Enter a valid salary:");
            }

            Employee emp = new Employee(id, age, name, salary);
            employeeDirectory[id] = emp;

            Console.WriteLine("Employee added successfully.\n");
        }

        public void PrintAllEmployees()
        {
            if (employeeDirectory.Count == 0)
            {
                Console.WriteLine("No employees found.\n");
                return;
            }

            Console.WriteLine("\n-------------------------------------");
            foreach (var emp in employeeDirectory.Values)
            {
                Console.WriteLine(emp);
                Console.WriteLine("------------------------------------");
            }
        }

        public void PrintEmployeeById()
        {
            Console.Write("Enter employee ID to search:");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("Invalid input. Please check the id.");
            }
            
            if (employeeDirectory.TryGetValue(id, out Employee emp))
            {
                Console.WriteLine("Employee details:\n" + emp);
            }
            else
            {
                Console.WriteLine($"Employee with ID {id} not found.");
            }
            
            
        }

        public void ModifyEmployee()
        {
            int id,age;
            string name;
            double salary;
            Console.Write("Enter the employee ID to modify: ");
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("Invalid ID. Try again.");
            }

            if (employeeDirectory.TryGetValue(id, out Employee emp))
            {

                
                name = GetNonEmptyString("Enter new name: ");

                Console.Write("Enter new age: ");
                while (!int.TryParse(Console.ReadLine(), out age) || age <= 0)
                {
                    Console.WriteLine("Invalid input. Enter a positive integer for age:");
                }

                Console.Write("Enter new salary: ");
                while (!double.TryParse(Console.ReadLine(), out salary) || salary < 0)
                {
                    Console.WriteLine("Invalid input. Enter a valid salary:");
                }

                emp.Name = name;
                emp.Age = age;
                emp.Salary = salary;

                Console.WriteLine("Employee updated.");
            }
            else
            {
                Console.WriteLine($"Employee with ID {id} does not exist.");
            }

        }

        public void DeleteEmployee()
        {
            int id;
            Console.Write("Enter the ID of the employee to delete: ");
            while (!int.TryParse(Console.ReadLine(), out id) || id <= 0)
            {
                Console.WriteLine("Invalid input. Enter a valid ID");
            }
            
            if (employeeDirectory.Remove(id))
            {
                Console.WriteLine($"Employee with ID {id} deleted.");
            }
            else
            {
                Console.WriteLine($"Employee with ID {id} not found.");
            }
            
        }

        static void showMenu()
        {
            Console.WriteLine("\n---------- Menu ------------");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Print All Employees");
            Console.WriteLine("3. Print Employee by ID");
            Console.WriteLine("4. Modify Employee");
            Console.WriteLine("5. Delete Employee");
            Console.WriteLine("6. Exit");
            Console.WriteLine("--------------------------------");
        }
        public void Run()
        {
            string? choice;
            while (true)
            {

                Console.Write("Choose an option: ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddEmployee();
                        break;
                    case "2":
                        PrintAllEmployees();
                        break;
                    case "3":
                        PrintEmployeeById();
                        break;
                    case "4":
                        ModifyEmployee();
                        break;
                    case "5":
                        DeleteEmployee();
                        break;
                    case "6":
                        Console.WriteLine("Exit");
                        return;

                    default:
                        Console.WriteLine("Invalid option. Please choose a number between 0 and 5.");
                        break;
                }
            }
        }
    }

}