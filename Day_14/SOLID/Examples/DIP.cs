using SOLID.interfaces;
using SOLID.repositories;
using SOLID.service;

// Bad Example
/*
    var registrationServices = new RegistrationService(); // Depends on concrete class
    var loginServices = new LoginService(); // Depends on concrete class

(Both objects initaialize user repository inside their class (Tightly coupled) )
*/


// Good Example

IUserRepository userRepository = new UserRepository();
IRegistrationService registrationService = new RegistrationService(userRepository);
ILoginService loginService = new LoginService(userRepository);