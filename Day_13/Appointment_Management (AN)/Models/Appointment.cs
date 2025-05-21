using System.Globalization;
using System.Xml.Linq;

namespace WholeApplication2.Models
{
    public class Appointment : IEquatable<Appointment>
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;

        public Appointment() { }

        public Appointment(int id, string name, int age, DateTime date, string reason)
        {
            Id = id;
            PatientName = name;
            PatientAge = age;
            AppointmentDate = date;
            Reason = reason;
        }

        public void TakeAppointmentDetailsFromUser()
        {

            Console.WriteLine("Please enter the Patient name");
            PatientName = Console.ReadLine() ?? "";
            Console.WriteLine("Please enter the Patient age");

            int age;
            while (!int.TryParse(Console.ReadLine(), out age) || age < 0)
            {
                Console.WriteLine("Invalid input. Please enter a positive integer.");
            }
            PatientAge = age;

            Console.WriteLine("Please enter date of appointment in (dd-MM-yyyy HH:mm): ");
            DateTime date;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) || date < DateTime.Now)
            {
                Console.WriteLine("Invalid entry for date. Please enter a valid date");
            }
            AppointmentDate = date;

            Console.WriteLine("Please enter the reason: ");
            Reason = Console.ReadLine() ?? "";

        }

        public override string ToString()
        {
            return "Appointment ID : " + Id + "\nPatient Name : " + PatientName + "\nPatient Age : " + PatientAge + "\nAppointment Date & Time " + AppointmentDate.ToString("dd-MM-yyyy HH:mm");
        }

        public bool Equals(Appointment? other)
        {
            return this.Id == other?.Id;
        }
    }
}
