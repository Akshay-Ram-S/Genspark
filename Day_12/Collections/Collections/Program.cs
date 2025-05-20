using Easy;
using Hard;
using Medium;
using check;
class Program
{
    static void showMenu()
    {
        Console.WriteLine("\n------------- MENU ----------------");
        Console.WriteLine("1. EmployeePromotion (Easy)");
        Console.WriteLine("2. EmployeeAggregate (Medium)");
        Console.WriteLine("3. EmployeeApp (Hard)");
        Console.WriteLine("4. Exit");
        Console.WriteLine("--------------------------------------");
    }
    static void Main()
    {
        string? choice;
        while (true)
        {
            showMenu();
            Console.Write("Please enter your choice: ");
            choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    var ep = new EmployeePromotion();
                    ep.Run();
                    break;

                case "2":
                    var ma = new EmployeeAggregate();
                    ma.Run();
                    break;

                case "3":
                    var ea = new EmployeeApp();
                    ea.Run();
                    break;

                case "4":
                    return;

                default:
                    Console.WriteLine("Invalid input. Enter a number between 1-4");
                    break;
            }
        }
    }

}