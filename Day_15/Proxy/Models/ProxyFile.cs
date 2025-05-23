using Proxy.interfaces;

namespace Proxy.models
{
    public class ProxyFile : IFile
    {
        File file;
        User _user;
        public ProxyFile(string path, User user)
        {
            file = new File(path);
            _user = user;
        }
        public void Read()
        {
            if (_user.role == "Admin")
            {
                Console.Write("[Access Granted] ");
                file.Read();
            }
            else if (_user.role == "User")
            {
                Console.Write("You have the permission to view metadata.");
                file.ReadMetaData();
            }
            else
            {
                Console.WriteLine("[Access Denied] You do not have permission to read this file.");
            }
        }
    }
}