using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;

namespace ERP.BusinessLogic.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task CreateAsync_HashesPasswordAndAssignsRole()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole("Manager");
            var service = new UserService(context);

            var user = await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Elena",
                LastName = "Petrova",
                Email = "elena@test.local",
                Password = "Secret@123",
                RoleId = role.Id
            });

            Assert.NotEqual("Secret@123", user.PasswordHash);
            Assert.NotEmpty(user.PasswordHash);
            Assert.Single(user.UserRoles);
            Assert.Equal(role.Id, user.UserRoles.Single().RoleId);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsUserForCorrectPassword()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole();
            var service = new UserService(context);
            await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = role.Id
            });

            var result = await service.AuthenticateAsync("georgi@test.local", "Secret@123");

            Assert.NotNull(result);
            Assert.Equal("georgi@test.local", result!.Email);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNullForWrongPassword()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole();
            var service = new UserService(context);
            await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = role.Id
            });

            var result = await service.AuthenticateAsync("georgi@test.local", "WrongPassword");

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNullForUnknownEmail()
        {
            using var context = TestDbContextFactory.Create();
            var service = new UserService(context);

            var result = await service.AuthenticateAsync("nobody@test.local", "Secret@123");

            Assert.Null(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsNullForInactiveUser()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole();
            var service = new UserService(context);
            var user = await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = role.Id,
                IsActive = false
            });

            var result = await service.AuthenticateAsync("georgi@test.local", "Secret@123");

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_BlankPassword_KeepsExistingPasswordWorking()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole();
            var service = new UserService(context);
            var user = await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = role.Id
            });

            await service.UpdateAsync(new UpdateUserRequest
            {
                Id = user.Id,
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = null,
                RoleId = role.Id
            });

            var result = await service.AuthenticateAsync("georgi@test.local", "Secret@123");
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_NewPassword_InvalidatesOldPassword()
        {
            using var context = TestDbContextFactory.Create();
            var role = context.SeedRole();
            var service = new UserService(context);
            var user = await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = role.Id
            });

            await service.UpdateAsync(new UpdateUserRequest
            {
                Id = user.Id,
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "NewSecret@456",
                RoleId = role.Id
            });

            Assert.Null(await service.AuthenticateAsync("georgi@test.local", "Secret@123"));
            Assert.NotNull(await service.AuthenticateAsync("georgi@test.local", "NewSecret@456"));
        }

        [Fact]
        public async Task UpdateAsync_ChangingRole_ReplacesExistingAssignment()
        {
            using var context = TestDbContextFactory.Create();
            var employeeRole = context.SeedRole("Employee");
            var managerRole = context.SeedRole("Manager");
            var service = new UserService(context);
            var user = await service.CreateAsync(new CreateUserRequest
            {
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                Password = "Secret@123",
                RoleId = employeeRole.Id
            });

            await service.UpdateAsync(new UpdateUserRequest
            {
                Id = user.Id,
                FirstName = "Georgi",
                LastName = "Dimitrov",
                Email = "georgi@test.local",
                RoleId = managerRole.Id
            });

            var updated = await service.GetByIdAsync(user.Id);
            var role = Assert.Single(updated!.UserRoles);
            Assert.Equal(managerRole.Id, role.RoleId);
        }
    }
}
