namespace Proxy.models
{
    public class User
    {
        public string? username { get; set; }
        public string? role { get; set; }

        public void GetUserDetailsAsInput()
        {
            Console.Write("User: ");
            username = Console.ReadLine();

            Console.WriteLine("Choose your role: (Admin, User, Guest):");

            Console.Write("Role: ");
            role = Console.ReadLine();
        }
    }

}