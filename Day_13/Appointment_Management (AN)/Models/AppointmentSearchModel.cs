namespace WholeApplication2.Models
{
    public class AppointmentSearchModel
    {
        public string? PatientName { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public Range<int>? AgeRange { get; set; }

       
    }
    public class Range<T>
    {
        public T? MinVal { get; set; }
        public T? MaxVal { get; set; }
    }
}
