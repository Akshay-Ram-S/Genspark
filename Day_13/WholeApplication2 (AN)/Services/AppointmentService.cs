using WholeApplication2.Interfaces;
using WholeApplication2.Models;

namespace WholeApplication2.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IRepository<int, Appointment> _appointmentRepository;

        public AppointmentService(IRepository<int, Appointment> appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public int AddAppointment(Appointment appointment)
        {
            try
            {
                var result = _appointmentRepository.Add(appointment);
                return result?.Id ?? -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel)
        {
            try
            {
                var appointments = _appointmentRepository.GetAll();

                appointments = FilterByName(appointments, searchModel.PatientName);
                appointments = FilterByAge(appointments, searchModel.AgeRange);
                appointments = FilterByDate(appointments, searchModel.AppointmentDate);

                return appointments?.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private ICollection<Appointment> FilterByDate(ICollection<Appointment> appointments, DateTime? date)
        {
            if (date == null || appointments == null || appointments.Count == 0)
                return appointments;

            return appointments.Where(a => a.AppointmentDate.Date == date.Value.Date).ToList();

        }

        private ICollection<Appointment> FilterByAge(ICollection<Appointment> appointments, Range<int>? ageRange)
        {
            if (ageRange == null || appointments == null || appointments.Count == 0)
                return appointments;

            return appointments.Where(a => a.PatientAge >= ageRange.MinVal && a.PatientAge <= ageRange.MaxVal).ToList();
        }

        private ICollection<Appointment> FilterByName(ICollection<Appointment> appointments, string? name)
        {
            if (string.IsNullOrWhiteSpace(name) || appointments == null || appointments.Count == 0)
                return appointments;

            var lowerName = name.ToLower();
            return appointments.Where(a => a.PatientName.ToLower().Contains(lowerName)).ToList();
        }
    }
}
