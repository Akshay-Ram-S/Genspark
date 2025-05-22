using System;
using System.Collections.Generic;
using SOLID.models;
using SOLID.service;
using SOLID.interfaces;
using SOLID.repositories;

namespace SOLID
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserRepository userRepository = new UserRepository();
            IRegistrationService registrationService = new RegistrationService(userRepository);
            ILoginService loginService = new LoginService(userRepository);

            registrationService.Register("akshay", "akshay123");
            registrationService.Register("john", "john123");

            Console.Write("Please enter username for login: ");
            string? loginUsername = Console.ReadLine();

            Console.Write("Please enter password for login: ");
            string? loginPassword = Console.ReadLine();

            if (loginService.Login(loginUsername, loginPassword))
            {
                Console.WriteLine("Login successful!");
            }
            else
            {
                Console.WriteLine("Invalid username or password.");
            }
        }
    }

}


    



    

    

    

    


    




