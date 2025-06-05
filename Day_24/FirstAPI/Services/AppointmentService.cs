using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;

namespace FirstAPI.Services
{
    class AppointmentService : IAppointmentService
    {
        AppointmentMapper appointmentMapper;

        private readonly IRepository<string, Appointmnet> _appointmnetRepository;

        public AppointmentService(IRepository<string, Appointmnet> appointmentRepository)
        {
            appointmentMapper = new();
            _appointmnetRepository = appointmentRepository;
        }
        public async Task<Appointmnet> BookAppointment(AppointmentFixDto appointment)
        {
            try
            {
                var newAppointment = appointmentMapper.MapAppointmentFix(appointment);
                newAppointment = await _appointmnetRepository.Add(newAppointment); 
                return newAppointment;
            }
            catch (Exception e)
            {
                throw new Exception("Appointment fix failed");    
            }
        }

        public async Task<Appointmnet> CancelAppointment(string key)
        {
            try
            {

                var appointment = await _appointmnetRepository.Get(key);
                if (appointment == null)
                {
                    throw new Exception($"No appointment found with appointment number: {key}");
                }
                appointment.Status = "Cancelled";
                var cancelAppointment = await _appointmnetRepository.Update(key, appointment);
                return cancelAppointment;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}