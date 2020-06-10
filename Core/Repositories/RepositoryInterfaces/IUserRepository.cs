using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task AddUserAsync(ApplicationUser user);
        bool IsExists(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(int userId);
        Task DeactivateUserAsync(ApplicationUser user);
        Task ActivateUserAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByUserNameAsync(string username);
        Task DeleteUserAsync(ApplicationUser user);
        string FindCurrentUserRole();
    }
}
