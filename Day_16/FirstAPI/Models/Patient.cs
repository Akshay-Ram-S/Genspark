namespace FirstAPI.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Phone { get; set; } = string.Empty;

    }

}