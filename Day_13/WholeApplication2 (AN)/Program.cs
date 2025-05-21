using WholeApplication2.Interfaces;
using WholeApplication.Repositories;
using WholeApplication2.Models;
using WholeApplication2.Services;


namespace WholeApplication2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            IRepository<int, Appointment> appointmentRepository = new EmployeeRepository();
            IAppointmentService appointmentService = new AppointmentService(appointmentRepository);
            ManageAppointment manageEmployee = new ManageAppointment(appointmentService);
            manageEmployee.Start();
        }
    }
}