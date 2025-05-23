using System.Dynamic;
using Proxy.interfaces;
using Proxy.models;

class Program
{
    static void Main()
    {
        User user = new User();
        user.GetUserDetailsAsInput();

        IFile file = new ProxyFile("sample.txt", user);
        file.Read();
    }
}
