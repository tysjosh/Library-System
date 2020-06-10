using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GraceChapelLibraryWebApp.Core.Enumerations;


namespace GraceChapelLibraryWebApp.Core.Models
{
    public class BookUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int Id { get; set; }

        [Required(ErrorMessage = "PhoneNumber required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Member name required")]
        public string FullName { get; set; }
        
        [Required(ErrorMessage = "UserName required")]
        public string UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "BookUser Role required")]
        public string Role { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password{get; set;}

        public UserStatus Status { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
