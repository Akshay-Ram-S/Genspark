using System.Globalization;
using WholeApplication2.Interfaces;
using WholeApplication2.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WholeApplication2
{
    public class ManageAppointment
    {
        private readonly IAppointmentService _appointmentService;

        public ManageAppointment(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        public void Start()
        {

            while (true)
            {
                PrintMenu();

                int option;
                while (!int.TryParse(Console.ReadLine(), out option) || (option < 1 || option > 2))
                {
                    Console.WriteLine("Invalid input. Please choose a valid option.");
                }

                switch (option)
                {
                    case 1:
                        AddAppointment();
                        break;
                    case 2:
                        SearchAppointment();
                        break;
                    default:
                        return;
                }
            }
        }

        public void PrintMenu()
        {
            Console.WriteLine("\n------------ Menu ------------");
            Console.WriteLine("1. Add Appointment");
            Console.WriteLine("2. Search Appointment");
            Console.WriteLine("Press any other key to exit");
        }

        public void AddAppointment()
        {
            var appointment = new Appointment();
            appointment.TakeAppointmentDetailsFromUser();
            int id = _appointmentService.AddAppointment(appointment);
            Console.WriteLine($"Appointment created successfully. Appointment ID: {id}");
        }

        public void SearchAppointment()
        {
            var searchCriteria = PrintSearchMenu();
            var results = _appointmentService.SearchAppointment(searchCriteria);

            Console.WriteLine("Search Results:");
            if (results == null || results.Count == 0)
            {
                Console.WriteLine("No appointments found for the given criteria.");
                return;
            }

            PrintAppointments(results);
        }

        private void PrintAppointments(List<Appointment>? appointments)
        {
            foreach (var appointment in appointments)
            {
                Console.WriteLine(appointment);
            }
        }

        private AppointmentSearchModel PrintSearchMenu()
        {
            var searchModel = new AppointmentSearchModel();
            int userInput;

            Console.WriteLine("Would you like to search by Name? Enter 1 for Yes, 2 for No:");
            while (!int.TryParse(Console.ReadLine(), out userInput) || (userInput != 1 && userInput != 2))
            {
                Console.WriteLine("Invalid input. Please enter 1 (Yes) or 2 (No).");
            }

            if (userInput == 1)
            {
                Console.WriteLine("Enter Patient Name:");
                searchModel.PatientName = Console.ReadLine() ?? string.Empty;
            }

            Console.WriteLine("Would you like to search by Age? Enter 1 for Yes, 2 for No:");
            while (!int.TryParse(Console.ReadLine(), out userInput) || (userInput != 1 && userInput != 2))
            {
                Console.WriteLine("Invalid input. Please enter 1 (Yes) or 2 (No).");
            }

            if (userInput == 1)
            {
                searchModel.AgeRange = new Range<int>();

                int minAge, maxAge;
                Console.WriteLine("Enter minimum Patient Age:");
                while (!int.TryParse(Console.ReadLine(), out  minAge) || minAge <= 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid age.");
                }
                searchModel.AgeRange.MinVal = minAge;

                Console.WriteLine("Enter maximum Patient Age:");
                while (!int.TryParse(Console.ReadLine(), out maxAge) || maxAge <= 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid age.");
                }
                searchModel.AgeRange.MaxVal = maxAge;
            }

            Console.WriteLine("Would you like to search by Date? Enter 1 for Yes, 2 for No:");
            while (!int.TryParse(Console.ReadLine(), out userInput) || (userInput != 1 && userInput != 2))
            {
                Console.WriteLine("Invalid input. Please enter 1 (Yes) or 2 (No).");
            }

            if (userInput == 1)
            {
                Console.WriteLine("Enter the date and time (format: dd-MM-yyyy):");
                while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime appointmentDate))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                }
                
            }

            return searchModel;
        }
    }
}
