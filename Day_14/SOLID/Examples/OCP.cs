// Bad Example
public class UserService
{
    public bool Login(string username, string password, string loginType)
    {
        // Logic 
        if (loginType == "App")
        {
            return true;
        }
        else if (loginType == "Webiste")
        {
            return true;
        }
        return false;
    }
}


// Good Example

public interface ILoginType
{
    bool Login(string username, string password);
}

public class AppLogin : ILoginType
{
    public bool Login(string username, string password)
    {
        // Logic
        return true;
    }
}

public class WebLogin : ILoginType
{
    public bool Login(string username, string password)
    {
        // Logic
        return true;
    }
}