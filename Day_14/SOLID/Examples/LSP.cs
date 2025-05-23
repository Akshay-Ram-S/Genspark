// Bad Example
public class UserServices
{
    public virtual void RegisterUser(string username, string password)
    {
        Console.WriteLine("User registered.");
    }
}

public class ReadOnlyUserService : UserServices
{
    public override void RegisterUser(string username, string password)
    {
        throw new NotImplementedException(); // LSP violation
    }
}

// Good Example


