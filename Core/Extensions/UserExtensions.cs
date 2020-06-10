using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Extensions
{
    public static class UserExtensions
    {
        public static void Map(this ApplicationUser dbUser, ApplicationUser user)
        {
            dbUser.FullName = user.FullName;
            dbUser.Status = user.Status;
            dbUser.Email = user.Email;
            dbUser.Role = user.Role;
            dbUser.UserName = user.UserName;
        }
    }
}
