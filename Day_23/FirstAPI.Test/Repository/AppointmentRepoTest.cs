using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Test
{
    public class AppointmnetRepositoryTests
    {
        private ClinicContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: "MyDb") 
                .Options;

            _context = new ClinicContext(options);
        }

        private Appointmnet CreateTestAppointment(int patientId = 1, int doctorId = 1)
        {
            return new Appointmnet
            {
                AppointmnetNumber = $"APT-{Guid.NewGuid()}",
                PatientId = patientId,
                DoctorId = doctorId,
                AppointmnetDateTime = DateTime.Now.AddDays(1),
                Status = "Success"
            };
        }

        [Test]
        public async Task AddAppointmnetTest()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);
            var appointment = CreateTestAppointment();

            var result = await repo.Add(appointment);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("Success"));
            Assert.That(result.AppointmnetNumber, Is.EqualTo(appointment.AppointmnetNumber));
        }

        [Test]
        public async Task GetAppointmnetTest()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);
            var appointment = await repo.Add(CreateTestAppointment());

            var result = await repo.Get(appointment.AppointmnetNumber);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.PatientId, Is.EqualTo(appointment.PatientId));
        }

        [Test]
        public async Task GetAllAppointmnetsTest()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);
            await repo.Add(CreateTestAppointment());
            await repo.Add(CreateTestAppointment(2, 2));

            var results = await repo.GetAll();

            Assert.That(results.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateAppointmnetTest()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);
            var appoint = await repo.Add(CreateTestAppointment());

            appoint.Status = "Completed";
            var updated = await repo.Update(appoint.AppointmnetNumber, appoint);

            Assert.That(updated.Status, Is.EqualTo("Completed"));
        }

        [Test]
        public async Task DeleteAppointmnetTest()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);
            var appoint = await repo.Add(CreateTestAppointment());

            var deleted = await repo.Delete(appoint.AppointmnetNumber);

            Assert.That(deleted.AppointmnetNumber, Is.EqualTo(appoint.AppointmnetNumber));

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Get(appoint.AppointmnetNumber));
            Assert.That(ex.Message, Is.EqualTo("No appointment with the given ID"));
        }

        [Test]
        public void DeleteAppointmnet_NotFound_ThrowsException()
        {
            IRepository<string, Appointmnet> repo = new AppointmnetRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await repo.Delete("non_existent_id"));

            Assert.That(ex.Message, Is.EqualTo("No appointment with the given ID"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
