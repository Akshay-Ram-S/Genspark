
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public  class AppointmentRepository : Repository<string, Appointment>
    {
        public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Appointment> Get(string key)
        {
            var patient = await _clinicContext.Appointments.SingleOrDefaultAsync(ap => ap.AppointmentNumber == key);

            return patient??throw new Exception("No Appointment with the given ID");
        }

        public override async Task<IEnumerable<Appointment>> GetAll()
        {
            var appointments = _clinicContext.Appointments;
            if (appointments.Count() == 0)
                throw new Exception("No Appointments in the database");

            return await appointments.ToListAsync();
        }
    }
}
