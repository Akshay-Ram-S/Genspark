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
    public class DoctorSpecialityTest
    {
        private ClinicContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ClinicContext(options);
        }

        private Doctor CreateTestDoctor(string name = "Test")
        {
            return new Doctor
            {
                Name = name,
                Email = $"test@gmail.com",
                Status = "Active"
            };
        }

        private Speciality CreateTestSpeciality(string name = "Cardiology")
        {
            return new Speciality
            {
                Name = name,
                Status = "Active"
            };
        }

        [Test]
        public async Task AddDoctorSpeciality_ShouldSucceed()
        {
            var doctor = _context.Doctors.Add(CreateTestDoctor()).Entity;
            var speciality = _context.Specialities.Add(CreateTestSpeciality()).Entity;
            await _context.SaveChangesAsync();

            var repo = new DoctorSpecialityRepository(_context);
            var ds = new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = speciality.Id };

            var result = await repo.Add(ds);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.DoctorId, Is.EqualTo(doctor.Id));
            Assert.That(result.SpecialityId, Is.EqualTo(speciality.Id));
        }

        [Test]
        public async Task GetDoctorSpeciality_ShouldReturnCorrectItem()
        {
            var doctor = _context.Doctors.Add(CreateTestDoctor()).Entity;
            var speciality = _context.Specialities.Add(CreateTestSpeciality()).Entity;
            await _context.SaveChangesAsync();

            var repo = new DoctorSpecialityRepository(_context);
            var added = await repo.Add(new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = speciality.Id });

            var result = await repo.Get(added.SerialNumber);

            Assert.That(result.SerialNumber, Is.EqualTo(added.SerialNumber));
        }

        [Test]
        public void GetDoctorSpeciality_InvalidId_ShouldThrow()
        {
            var repo = new DoctorSpecialityRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Get(999));
            Assert.That(ex.Message, Is.EqualTo("No DoctorSpeciality with the given ID"));
        }

        [Test]
        public async Task GetAllDoctorSpecialities_ShouldReturnCorrectCount()
        {
            var doctor = _context.Doctors.Add(CreateTestDoctor()).Entity;
            var spec1 = _context.Specialities.Add(CreateTestSpeciality("Cardiology")).Entity;
            var spec2 = _context.Specialities.Add(CreateTestSpeciality("Neurology")).Entity;
            await _context.SaveChangesAsync();

            var repo = new DoctorSpecialityRepository(_context);
            await repo.Add(new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = spec1.Id });
            await repo.Add(new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = spec2.Id });

            var result = await repo.GetAll();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task UpdateDoctorSpeciality_ShouldUpdateSpecialityId()
        {
            var doctor = _context.Doctors.Add(CreateTestDoctor()).Entity;
            var spec1 = _context.Specialities.Add(CreateTestSpeciality("Ortho")).Entity;
            var spec2 = _context.Specialities.Add(CreateTestSpeciality("Neuro")).Entity;
            await _context.SaveChangesAsync();

            var repo = new DoctorSpecialityRepository(_context);
            var ds = await repo.Add(new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = spec1.Id });

            ds.SpecialityId = spec2.Id;
            var updated = await repo.Update(ds.SerialNumber, ds);

            Assert.That(updated.SpecialityId, Is.EqualTo(spec2.Id));
        }

        [Test]
        public void UpdateDoctorSpeciality_InvalidId_ShouldThrow()
        {
            var repo = new DoctorSpecialityRepository(_context);
            var fakeDS = new DoctorSpeciality { SerialNumber = 999, DoctorId = 1, SpecialityId = 1 };

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Update(999, fakeDS));
            Assert.That(ex.Message, Is.EqualTo("No DoctorSpeciality with the given ID"));
        }

        [Test]
        public async Task DeleteDoctorSpeciality_ShouldSucceed()
        {
            var doctor = _context.Doctors.Add(CreateTestDoctor()).Entity;
            var speciality = _context.Specialities.Add(CreateTestSpeciality()).Entity;
            await _context.SaveChangesAsync();

            var repo = new DoctorSpecialityRepository(_context);
            var ds = await repo.Add(new DoctorSpeciality { DoctorId = doctor.Id, SpecialityId = speciality.Id });

            var deleted = await repo.Delete(ds.SerialNumber);

            Assert.That(deleted.SerialNumber, Is.EqualTo(ds.SerialNumber));
        }

        [Test]
        public void DeleteDoctorSpeciality_InvalidId_ShouldThrow()
        {
            var repo = new DoctorSpecialityRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Delete(999));
            Assert.That(ex.Message, Is.EqualTo("No DoctorSpeciality with the given ID"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
