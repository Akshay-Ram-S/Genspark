// Bad Example
public interface IUserService
{
    public void Register(string username, string password);
    public bool Login(string username, string password);
    public bool Validate(string username, string password) ;
}

// Good Example

public interface IRegistrationService
{
    void Register(string username, string password);
}

public interface ILoginService
{
    bool Login(string username, string password);
}