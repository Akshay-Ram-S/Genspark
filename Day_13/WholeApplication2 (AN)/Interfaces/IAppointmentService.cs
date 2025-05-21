
using WholeApplication2.Models;

namespace WholeApplication2.Interfaces
{
    public interface IAppointmentService
    {
        int AddAppointment(Appointment appointment);
        List<Appointment>? SearchAppointment(AppointmentSearchModel searchModel);

    }
}
