using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Interfaces;

namespace FirstAPI.Test;
public class Tests
{
    ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

        _context = new ClinicContext(options);
        
    }

    private Doctor CreateTestDoctor(string email = "test@gmail.com")
    {
        return new Doctor
        {
            Name = "Test Doctor",
            YearsOfExperience = 5,
            Email = email
        };
    }

    [Test]
    public async Task AddDoctorTest()
    {
        //arrange
        var email = "test@gmail.com";
        var password = "123456";
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            Role = "Doctor"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var doctor = CreateTestDoctor(email);
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        //action
        var result = await _doctorRepository.Add(doctor);
        //assert
        Assert.That(result, Is.Not.Null, "Doctor IS not addeed");
        Assert.That(result.Id, Is.EqualTo(1));
        Assert.That(result.Email, Is.EqualTo("testt@gmail.com"));
    }

    [TestCase(1)]
    [TestCase(2)]
    public async Task GetDoctorPassTest(int id)
    {
        //arrange
        var email = "test123@gmail.com";
        var password = "123";
        var key = Guid.NewGuid().ToByteArray();
        var user = new User
        {
            Username = email,
            Password = password,
            Role = "Doctor"
        };
        _context.Add(user);
        await _context.SaveChangesAsync();
        var doctor = CreateTestDoctor(email);
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        //action
        await _doctorRepository.Add(doctor);

        //action
        var result = _doctorRepository.Get(id);
        //assert
        Assert.That(result.Id, Is.EqualTo(id));

    }

    // [TestCase(3)]
    // public async Task GetDoctorExceptionTest(int id)
    // {
    //     // Arrange
    //     var email = "test@gmail.com";
    //     var password = System.Text.Encoding.UTF8.GetBytes("test123");
    //     var key = Guid.NewGuid().ToByteArray();

    //     var user = new User
    //     {
    //         Username = email,
    //         Password = password,
    //         HashKey = key,
    //         Role = "Doctor"
    //     };
    //     _context.Users.Add(user);
    //     await _context.SaveChangesAsync();

    //     var doctor = new Doctor
    //     {
    //         Name = "test",
    //         YearsOfExperience = 2,
    //         Email = email
    //     };
    //     IRepository<int, Doctor> doctorRepository = new DoctorRepository(_context);
    //     await doctorRepository.Add(doctor); // this gets ID = 1

    //     // Act & Assert
    //     var ex = await Assert.ThrowsAsync<Exception>(async () => await doctorRepository.Get(id));
    //     Assert.That(ex.Message, Is.EqualTo("No Doctor with the given ID"));
    // }

    [Test]
    public async Task GetAllDoctorsTest()
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

        var doctors = new List<Doctor>
        {
            CreateTestDoctor("doc1@gmail.com"),
            CreateTestDoctor("doc2@gmail.com"),
            CreateTestDoctor("doc3@gmail.com"),
        };

        foreach (var doc in doctors)
        {
            await _doctorRepository.Add(doc);
        }

        var result = await _doctorRepository.GetAll();
        var results = result.ToList();
        Assert.That(results.Count, Is.EqualTo(doctors.Count));
        Assert.That(results.ConvertAll(d => d.Email), Is.EquivalentTo(doctors.ConvertAll(d => d.Email)));
    }

    [Test]
    public async Task UpdateDoctorTest()
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        var doctor = await _doctorRepository.Add(CreateTestDoctor());

        doctor.Name = "Updated Name";
        doctor.YearsOfExperience = 10;

        var updatedDoctor = await _doctorRepository.Update(doctor.Id, doctor);

        Assert.That(updatedDoctor, Is.Not.Null);
        Assert.That(updatedDoctor.Id, Is.EqualTo(doctor.Id));
        Assert.That(updatedDoctor.Name, Is.EqualTo("Updated Name"));
        Assert.That(updatedDoctor.YearsOfExperience, Is.EqualTo(10));
    }

    [Test]
    public void UpdateDoctor_NotFound_ThrowsException()
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

        var doctor = CreateTestDoctor();
        doctor.Id = 999; 

        var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            await _doctorRepository.Update(doctor.Id, doctor));

        Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
    }


    [Test]
    public async Task DeleteDoctorTest()
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);
        var doctor = await _doctorRepository.Add(CreateTestDoctor());

        var deleteResult = await _doctorRepository.Delete(doctor.Id);

        Assert.That(deleteResult.Id, Is.EqualTo(doctor.Id));

        var ex = Assert.ThrowsAsync<System.Exception>(async () => await _doctorRepository.Get(doctor.Id));
        Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
    }


    [Test]
    public void DeleteDoctor_NotFound_ThrowsException()
    {
        IRepository<int, Doctor> _doctorRepository = new DoctorRepository(_context);

        var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            await _doctorRepository.Delete(1000)); 

        Assert.That(ex.Message, Is.EqualTo("No doctor with the given ID"));
    }


    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}