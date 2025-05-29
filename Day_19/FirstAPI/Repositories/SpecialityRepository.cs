
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public  class SpecialityRepository : Repository<int, Speciality>
    {
        public SpecialityRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<Speciality> Get(int key)
        {
            var patient = await _clinicContext.Specialities.SingleOrDefaultAsync(p => p.Id == key);

            return patient??throw new Exception("No speciality with teh given ID");
        }

        public override async Task<IEnumerable<Speciality>> GetAll()
        {
            var specialities = _clinicContext.Specialities;
            if (specialities.Count() == 0)
                throw new Exception("No Specialities in the database");

            return await specialities.ToListAsync();
        }
    }
}
