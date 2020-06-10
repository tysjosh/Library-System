using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Extensions
{
    public static class BorrowerExtensions
    {
        public static void Map(this Borrower dbBorrower, Borrower borrower)
        {
            dbBorrower.BookId = borrower.BookId;
            dbBorrower.BorrowedFromDate = borrower.BorrowedFromDate;
            dbBorrower.BorrowedToDate = borrower.BorrowedToDate;
            dbBorrower.FullName = borrower.FullName;
            dbBorrower.BorrowStatus = borrower.BorrowStatus;
        }
    }
}
