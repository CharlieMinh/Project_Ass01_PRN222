using Scientific.Entities.Models;
using Scientific.Repositories;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Scientific.Services
{
    public class UsersHuyDdSevice : IUsersHuyDdSevice
    {
        private readonly UserRepository _userRepository;

        public UsersHuyDdSevice()
            => _userRepository = new UserRepository();

        public async Task<UsersHuyDd?> GetUsersHuyDdAsync(string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            return await _userRepository.GetUsersHuyDdAsync(userName.Trim(), HashPassword(password));
        }

        public async Task<UsersHuyDd?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var user = await _userRepository.GetActiveUserWithRolesByEmailAsync(email.Trim());
            if (user == null)
            {
                return null;
            }

            return VerifyPassword(password, user.PasswordHash) ? user : null;
        }

        public async Task<UsersHuyDd> RegisterAsync(
            string fullName,
            string email,
            string password,
            string? phoneNumber,
            string? organization,
            string? academicTitle)
        {
            if (await EmailExistsAsync(email))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new UsersHuyDd
            {
                FullName = fullName.Trim(),
                Email = email.Trim(),
                PasswordHash = HashPassword(password),
                PhoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim(),
                Organization = string.IsNullOrWhiteSpace(organization) ? null : organization.Trim(),
                AcademicTitle = string.IsNullOrWhiteSpace(academicTitle) ? null : academicTitle.Trim(),
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            return await _userRepository.RegisterAsync(user, "LecturerStudent");
        }

        public async Task<UsersHuyDd?> GetByIdWithRolesAsync(int userId)
        {
            return await _userRepository.GetUsersHuyDdWithRolesAsync(userId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _userRepository.EmailExistsAsync(email.Trim());
        }

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                return false;
            }

            if (passwordHash.StartsWith("$2", StringComparison.Ordinal))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(password, passwordHash);
                }
                catch
                {
                    return false;
                }
            }

            return string.Equals(HashPassword(password), passwordHash, StringComparison.OrdinalIgnoreCase);
        }
    }
}
