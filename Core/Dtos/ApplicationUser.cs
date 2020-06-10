using System;
using System.ComponentModel.DataAnnotations.Schema;
using GraceChapelLibraryWebApp.Core.Enumerations;
using Microsoft.AspNetCore.Identity;

namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }

        [Column]
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Role { get; set; }
        public UserStatus Status { get; set; }
    }
}
