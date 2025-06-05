using FirstAPI.Models;
using FirstAPI.Models.DTOs;
namespace FirstAPI.Misc
{
    public class AppointmentMapper
    {
        public Appointmnet? MapAppointmentFix(AppointmentFixDto addRequestDto)
        {
            Appointmnet appointment = new();
            appointment.AppointmnetNumber = Guid.NewGuid().ToString();
            appointment.PatientId = addRequestDto.PatientId;
            appointment.DoctorId = addRequestDto.DoctorId;
            appointment.Status = "Active";
            appointment.AppointmnetDateTime = addRequestDto.AppointmnetDateTime;
            return appointment;
        }
    }
}