using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !user.IsActive)
                return null;

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return null;

            return user;
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> CreateAsync(CreateUserRequest request)
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                IsActive = request.IsActive
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            user.UserRoles.Add(new UserRole { RoleId = request.RoleId });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task UpdateAsync(UpdateUserRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == request.Id)
                ?? throw new InvalidOperationException($"User with Id {request.Id} was not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Phone = request.Phone;
            user.IsActive = request.IsActive;

            if (!string.IsNullOrWhiteSpace(request.Password))
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            if (!user.UserRoles.Any(ur => ur.RoleId == request.RoleId))
            {
                _context.UserRoles.RemoveRange(user.UserRoles);
                user.UserRoles.Add(new UserRole { RoleId = request.RoleId });
            }

            await _context.SaveChangesAsync();
        }
    }
}
