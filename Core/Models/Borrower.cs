using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Enumerations;

namespace GraceChapelLibraryWebApp.Core.Models
{
    public class Borrower
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Book can not be borrowed without Member id")]
        [Range(1, int.MaxValue, ErrorMessage = "Book can not be borrowed without Member id")]
        public int MemberId { get; set; }
        [Required(ErrorMessage = "Invalid Book Id")]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid Book Id")]
        [ForeignKey("BookId")]
        public int BookId { get; set; }
        [Required(ErrorMessage = "Full name required to borrow book")]
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public Book Book { set; get; }
        public BorrowStatus BorrowStatus { get; set; }
        [Required(ErrorMessage = "From Date required")]
        public DateTime BorrowedFromDate { get; set; }
        public int ExtendCount { get; set; }
        [Required(ErrorMessage = "To Date required")]
        public DateTime BorrowedToDate { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
