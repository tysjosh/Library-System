using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces
{
    public interface IBorrowerRepository
    {
        Task<IEnumerable<BorrowerDetailsDto>> GetAllBorrowersAsync(string statusFilter, string timeRangeFilter);
        //Task<IEnumerable<BorrowerDetailsDto>> GetBorrowerBySignumAsync(string signum);
        //Task<Borrower> GetBorrowerByEmployeeIdAndBookIdIdAsync(int empId, int bookId);
        Task<Borrower> GetBorrowerByIdAsync(int borrowerId);
        Task CreateBorrowerAsync(Borrower borrower);
        Task UpdateBorrowerAsync(Borrower dbBorrower, Borrower borrower);
        Task DeleteBorrowerAsync(Borrower borrower);
        Task ExtendBookAsync(Borrower borrower);
        Task ReturnBookAsync(Borrower borrower);
        Task UpdateStatusToExpired();
        Task MonthlyReportForAdmin();
        Task WeeklyReminderForOverdue();
        Task PreExpiryReminder();
        bool IsExists(Borrower borrower);
    }
}