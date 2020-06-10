using System;
using GraceChapelLibraryWebApp.Core.Enumerations;

namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class BorrowerDto
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public int BookId { get; set; }
        public string FullName { get; set; }
        public string Signum { get; set; }
        public string Email { get; set; }
        public BorrowStatus BorrowStatus { get; set; }
        public DateTime BorrowedFromDate { get; set; }
        public DateTime BorrowedToDate { get; set; }
    }


}
