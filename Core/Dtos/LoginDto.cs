using Microsoft.AspNetCore.Identity;

namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
