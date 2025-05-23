// Bad Example
public class SignInSerice
{
    public void LoginUser() { } // Login Service
    public bool Validate() { return true || false; } // Validate Service 
    public void RegisterUser() { } // Register service
}

// Good Example
public class LoginServices()
{
    public void login()
    {
        // Logic
    }
}

public class RegisterServices()
{
    public void register()
    {
        // Logic
    }
}

public class ValidateUser
{
    public bool Validate()
    {
        // Based on logic return true/false
        return true || false;
    }
}