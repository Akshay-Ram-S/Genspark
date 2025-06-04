using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using FirstAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FirstAPI.Test
{
    public class UserTest
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

        private User CreateTestUser(string username = "testuser@gmail.com")
        {
            return new User
            {
                Username = username,
                Password = "12345",
                Role = "Patient"
            };
        }

        [Test]
        public async Task AddUser_ShouldSucceed()
        {
            var repo = new UserRepository(_context);
            var user = CreateTestUser();

            var result = await repo.Add(user);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task GetUser_ShouldReturnCorrectUser()
        {
            var repo = new UserRepository(_context);
            var user = CreateTestUser();
            await repo.Add(user);

            var result = await repo.Get(user.Username);

            Assert.That(result.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public void GetUser_InvalidUsername_ShouldThrow()
        {
            var repo = new UserRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Get("unknown@gmail.com"));
            Assert.That(ex.Message, Is.EqualTo("No User with the given ID"));
        }

        [Test]
        public async Task GetAllUsers_ShouldReturnAll()
        {
            var repo = new UserRepository(_context);

            var users = new[]
            {
                CreateTestUser("user1@example.com"),
                CreateTestUser("user2@example.com"),
                CreateTestUser("user3@example.com")
            };

            foreach (var user in users)
                await repo.Add(user);

            var result = await repo.GetAll();
            var resultList = result.ToList();

            Assert.That(resultList.Count, Is.EqualTo(users.Length));
        }

        [Test]
        public async Task UpdateUser_ShouldSucceed()
        {
            var repo = new UserRepository(_context);
            var user = await repo.Add(CreateTestUser());

            user.Role = "Doctor";
            var updated = await repo.Update(user.Username, user);

            Assert.That(updated.Role, Is.EqualTo("Doctor"));
        }

        [Test]
        public void UpdateUser_InvalidUsername_ShouldThrow()
        {
            var repo = new UserRepository(_context);
            var fakeUser = CreateTestUser("invalid@abc.com");

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Update(fakeUser.Username, fakeUser));
            Assert.That(ex.Message, Is.EqualTo("No such item found for updation"));
        }

        [Test]
        public async Task DeleteUser_ShouldSucceed()
        {
            var repo = new UserRepository(_context);
            var user = await repo.Add(CreateTestUser());

            var deleted = await repo.Delete(user.Username);

            Assert.That(deleted.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public void DeleteUser_InvalidUsername_ShouldThrow()
        {
            var repo = new UserRepository(_context);

            var ex = Assert.ThrowsAsync<Exception>(async () => await repo.Delete("invalid@abc.com"));
            Assert.That(ex.Message, Is.EqualTo("No such item found for updation"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
