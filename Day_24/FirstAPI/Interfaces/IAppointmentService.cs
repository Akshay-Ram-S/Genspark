using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces
{
    public interface IAppointmentService
    {
        public Task<Appointmnet> BookAppointment(AppointmentFixDto appointment);
        public Task<Appointmnet> CancelAppointment(string id);
    }
}