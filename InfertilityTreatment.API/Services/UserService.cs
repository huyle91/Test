using Microsoft.EntityFrameworkCore;
using Test.Data;
using Test.Models;
using Test.Models.DTOs;

namespace Test.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            // Check if email exists
            if (await IsEmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Check if username exists
            if (await IsUsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Hash password (simple example - use proper hashing in production)
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }
    }
}