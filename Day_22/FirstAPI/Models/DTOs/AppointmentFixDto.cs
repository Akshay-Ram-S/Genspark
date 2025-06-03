namespace FirstAPI.Models.DTOs
{
    public class AppointmentFixDto
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmnetDateTime { get; set; }
    }
}