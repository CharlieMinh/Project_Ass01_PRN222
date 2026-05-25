using Microsoft.EntityFrameworkCore;
using Scientific.Entities.Models;
using System;
using System.Threading.Tasks;

namespace Scientific.Repositories
{
    public class UserRepository : GenericRepository<UsersHuyDd>
    {
        public UserRepository() : base()
        {
        }

        public UserRepository(ScientificJournalTrendDBContext context) : base(context)
        {
        }

        public async Task<UsersHuyDd?> GetUsersHuyDdAsync(string userName, string passwordHash)
        {
            return await _context.UsersHuyDds.FirstOrDefaultAsync(
                x => x.Email == userName && x.PasswordHash == passwordHash && x.IsActive);
        }

        public async Task<UsersHuyDd?> GetUsersHuyDdWithRolesAsync(string email, string passwordHash)
        {
            return await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.Email == email && x.PasswordHash == passwordHash && x.IsActive);
        }

        public async Task<UsersHuyDd?> GetActiveUserWithRolesByEmailAsync(string email)
        {
            return await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.Email == email && x.IsActive);
        }

        public async Task<UsersHuyDd?> GetUsersHuyDdWithRolesAsync(int userId)
        {
            return await _context.UsersHuyDds
                .Include(x => x.RoleIdHuyDds)
                .FirstOrDefaultAsync(x => x.UserIdHuyDd == userId);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.UsersHuyDds.AnyAsync(x => x.Email == email);
        }

        public async Task<UsersHuyDd> RegisterAsync(UsersHuyDd user, string roleName)
        {
            var role = await _context.RolesHuyDds
                .AsTracking()
                .FirstOrDefaultAsync(x => x.RoleName == roleName);

            if (role == null)
            {
                role = new RolesHuyDd
                {
                    RoleName = roleName,
                    Description = "Default registered user role",
                    CreatedAt = DateTime.Now
                };
                _context.RolesHuyDds.Add(role);
            }

            user.RoleIdHuyDds.Add(role);
            _context.UsersHuyDds.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
    }
}
