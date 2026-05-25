using Scientific.Entities.Models;
using System.Threading.Tasks;

namespace Scientific.Services
{
    public interface IUsersHuyDdSevice
    {
        Task<UsersHuyDd?> GetUsersHuyDdAsync(string userName, string password);
        Task<UsersHuyDd?> LoginAsync(string email, string password);
        Task<UsersHuyDd> RegisterAsync(string fullName, string email, string password, string? phoneNumber, string? organization, string? academicTitle);
        Task<UsersHuyDd?> GetByIdWithRolesAsync(int userId);
        Task<bool> EmailExistsAsync(string email);
    }
}
