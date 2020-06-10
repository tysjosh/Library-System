using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Enumerations;
using GraceChapelLibraryWebApp.Core.Extensions;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
    {
        private readonly BookLibraryContext _bookLibraryContext;
        private readonly IMapper _mapper;
        private List<ApplicationUser> _userEntity;
        private IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(BookLibraryContext repositoryContext, IEmailService emailService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
       : base(repositoryContext, emailService)
        {
            _mapper = mapper;
            _bookLibraryContext = repositoryContext;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            _userEntity = await _bookLibraryContext.ApplicationUsers.ToListAsync();
            var results = _mapper.Map<IEnumerable<ApplicationUser>>(_userEntity);
            return results;
        }

        public async Task ActivateUserAsync(ApplicationUser dbUser)
        {
            ApplicationUser userNew = dbUser;
            userNew.Status = UserStatus.Active;
            dbUser.Map(userNew);
            Update(dbUser);
            await SaveAsync();
        }

        public async Task DeactivateUserAsync(ApplicationUser dbUser)
        {
            ApplicationUser userNew = dbUser;
            userNew.Status = UserStatus.Inactive;
            dbUser.Map(userNew);
            Update(dbUser);
            await SaveAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(int userId)
        {
            var user = await FindByConditionAync(o => o.Id.Equals(userId));
            return user.FirstOrDefault();
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string username)
        {
            var User = await FindByConditionAync(o => o.UserName.Equals(username));
            return User.FirstOrDefault();
        }

        public bool IsExists(ApplicationUser user)
        {
            return _bookLibraryContext.ApplicationUsers.Any(u => u.Id == user.Id);
        }

        public async Task AddUserAsync(ApplicationUser user)
        {
            Create(user);
            await SaveAsync();
        }

        public async Task DeleteUserAsync(ApplicationUser user)
        {
            Delete(user);
            await SaveAsync();
        }

        public string FindCurrentUserRole()
        {
            var role = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role).Value;
            return role;
        }
    }
}
