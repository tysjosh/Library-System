using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Enumerations;
using GraceChapelLibraryWebApp.Core.Extensions;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using GraceChapelLibraryWebApp.Core.Templates;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public class BorrowerRepository : RepositoryBase<Borrower>, IBorrowerRepository

    {
        private readonly BookLibraryContext _context;
        private readonly IEmailService _emailService;
        private List<Borrower> _borrowerEntity;
        private List<ApplicationUser> _userEntity;
        private readonly IMapper _mapper;
        private readonly IEmailTemplate _emailTemplate;
        public BorrowerRepository(BookLibraryContext repositoryContext, IEmailService emailService, IEmailTemplate emailTemplate, IMapper mapper)
        : base(repositoryContext, emailService)
        {
            _mapper = mapper;
            _context = repositoryContext;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
        }

        public async Task CreateBorrowerAsync(Borrower borrower)
        {
            Create(borrower);
            await SaveAsync();
        }

        public async Task DeleteBorrowerAsync(Borrower borrower)
        {
            Delete(borrower);
            await SaveAsync();

        }

        public async Task ExtendBookAsync(Borrower dbBorrower)
        {
            Borrower borrowerNew = dbBorrower;
            borrowerNew.BorrowedToDate = DateTime.Now.AddMonths(1);
            borrowerNew.BorrowedFromDate = DateTime.Now;
            borrowerNew.BorrowStatus = BorrowStatus.Borrowed;
            borrowerNew.ExtendCount = dbBorrower.ExtendCount + 1;
            dbBorrower.Map(borrowerNew);
            Update(dbBorrower);
            await SaveAsync();
        }

        public async Task<IEnumerable<BorrowerDetailsDto>> GetAllBorrowersAsync(string statusFilter, string timeRangeFilter)
        {
            _borrowerEntity = await _context.Borrowers.Include(b => b.Book).ToListAsync();
            //filter with the status
            switch (statusFilter)
            {
                case "returned":
                    _borrowerEntity = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Returned).ToList();
                    break;
                case "overdue":
                    _borrowerEntity = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Overdue).ToList();
                    break;
                case "borrowed":
                    _borrowerEntity = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Borrowed).ToList();
                    break;
                default:
                    //if needed later 
                    break;
            }

            //filter with the timerange 
            switch (timeRangeFilter)
            {
                case "weekly":
                    DateTime startAtMonday = DateTime.Now.AddDays(DayOfWeek.Monday - DateTime.Now.DayOfWeek);
                    _borrowerEntity = _borrowerEntity.Where(b => (b.Created >= startAtMonday || b.Modified >= startAtMonday)).ToList();
                    break;
                case "monthly":
                    var startOfTthisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    _borrowerEntity = _borrowerEntity.Where(b => (b.Created >= startOfTthisMonth || b.Modified >= startOfTthisMonth)).ToList();
                    break;
                case "yearly":
                    DateTime firstDayCurrentYear = new DateTime(DateTime.Now.Year, 1, 1);
                    _borrowerEntity = _borrowerEntity.Where(b => (b.Created >= firstDayCurrentYear || b.Modified >= firstDayCurrentYear)).ToList();
                    break;
                default: // not sure about default
                    break;
            }
            var results = _mapper.Map<IEnumerable<BorrowerDetailsDto>>(_borrowerEntity);
            return results;
        }

        //public async Task<Borrower> GetBorrowerByEmployeeIdAndBookIdIdAsync(int empId, int bookId)
        //{
        //    return _context.Borrowers.Where(
        //        b => b.BookId == bookId && (b.BorrowStatus == BorrowStatus.Borrowed ||
        //             b.BorrowStatus == BorrowStatus.Overdue) && b.MemberId == empId)
        //             .FirstOrDefault();
        //}

        public async Task<Borrower> GetBorrowerByIdAsync(int borrowerId)
        {
            var Borrower = await FindByConditionAync(o => o.Id.Equals(borrowerId));
            return Borrower.FirstOrDefault();
        }

        //public async Task<IEnumerable<BorrowerDetailsDto>> GetBorrowerBySignumAsync(string signum)
        //{
        //    _borrowerEntity = await _context.Borrowers.Include(b => b.Book).Where(b => b.Signum == signum).ToListAsync();
        //    var results = _mapper.Map<IEnumerable<BorrowerDetailsDto>>(_borrowerEntity);
        //    return results;
        //}

        public bool IsExists(Borrower borrower)
        {
            return _context.Borrowers.Any(b => b.BookId == borrower.BookId && b.BorrowStatus == 0 && b.Id == borrower.Id);
        }

        public async Task ReturnBookAsync(Borrower dbBorrower)
        {
            Borrower borrowerNew = dbBorrower;
            borrowerNew.BorrowStatus = BorrowStatus.Returned;
            dbBorrower.Map(borrowerNew);
            Update(dbBorrower);
            await SaveAsync();
        }

        public async Task UpdateBorrowerAsync(Borrower dbBorrower, Borrower borrower)
        {
            dbBorrower.Map(borrower);
            Update(dbBorrower);
            await SaveAsync();
        }

        public async Task UpdateStatusToExpired()
        {
            // find all expired entry
            DateTime oldestDate = DateTime.Now.Subtract(new TimeSpan(30, 0, 0, 0, 0));
            _borrowerEntity = await _context.Borrowers.Include(b => b.Book).
                Where(b => b.BorrowedFromDate <= oldestDate && b.BorrowStatus == BorrowStatus.Borrowed).ToListAsync();

            // change the status to expired  

            _borrowerEntity.ForEach(b =>
            {
                b.BorrowStatus = BorrowStatus.Overdue;
                var email = b.Email;
                var subject = "Friendly Reminder from Grace Chapel Book Library";
                var message = $"Hello {b.FullName}, <br /> <br />The return date of the book <b>{b.Book.Title}</b> has expired. <br />Please renew the borrowed book or return it.<br />To renew the book please login using your Username and Password, and click on the extend button from the profile page.<br /> <br />Thank You <br /> Grace Chapel Library Admin";
                _emailService.SendEmail(email, subject, message);
            }
            );
            _context.Borrowers.UpdateRange(_borrowerEntity);
            await _context.SaveChangesAsync();

        }
        public async Task MonthlyReportForAdmin()
        {
            // monthly report for the admin about borrower(how many book borrowed, returned, overdued etc)
            // find the monthly entry(for the previous month as schedule will run first day of the month) 
            var startOfTthisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var firstDay = startOfTthisMonth.AddMonths(-1);
            var lastDay = startOfTthisMonth.AddDays(-1);
            _borrowerEntity = await _context.Borrowers.Include(b => b.Book).Where(
                b => (b.Created >= firstDay && b.Created <= lastDay) || (b.Modified >= firstDay && b.Modified <= lastDay)).ToListAsync();
            // separate the entry as borrowed, returned, overdued,
            var borrowedPastMonth = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Borrowed).ToList();
            var overduePastMonth = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Overdue).ToList();
            var returnedPastMonth = _borrowerEntity.Where(b => b.BorrowStatus == BorrowStatus.Returned).ToList();
            // make some list with email template 
            // find the admin user email from the database 
            _userEntity = await _context.ApplicationUsers.Where(u => u.Role == "Admin" && u.Status == UserStatus.Active).ToListAsync();
            _userEntity.ForEach(u =>
            {
                // send the email using u.Email with the data found few lines ago
                // currently sending the total number only 
                //later email template will be used using details information 
                var email = u.Email;
                var subject = "The Monthly Report from Grace Chapel Book Library";
                var message =  _emailTemplate.MakeTemplateForMonthlyReport(u, firstDay, borrowedPastMonth, returnedPastMonth, overduePastMonth);
                var result = message.Result.ToString();
                _emailService.SendEmail(email, subject, result);
            });
        }

        //if the book overdued then send reminder per week to return it 
        public async Task WeeklyReminderForOverdue()
        {
            // find all overdued borrowers 
            _borrowerEntity = await _context.Borrowers.Include(b => b.Book).Where(b => b.BorrowStatus == BorrowStatus.Overdue).ToListAsync();
            _borrowerEntity.ForEach(b =>
            {
                var email = b.Email;
                var subject = "Friendly Reminder from Grace Chapel Book Library";
                var message = $"Hello {b.FullName}, <br /> <br />The return date of the book <b>{b.Book.Title}</b> has been overdue. <br />Please renew the borrowed book or return it.<br />To renew the book please login using your Username and Password, and click on the extend button from the profile page.<br /> <br />Thank You <br /> Grace Chapel Library Admin";
                _emailService.SendEmail(email, subject, message);
            }
            );
        }

        public async Task PreExpiryReminder()
        {
            
            //find books those are goind to expire exactly in five days 
            _borrowerEntity = await _context.Borrowers.Include(b => b.Book)
                .Where(b => (Math.Ceiling((b.BorrowedToDate - DateTime.Now).TotalDays)) == 5 &&
                            b.BorrowStatus == BorrowStatus.Borrowed).ToListAsync();
            //Send email to those users as pre-expire reminder 
            _borrowerEntity.ForEach(b =>
            {
                b.BorrowStatus = BorrowStatus.Overdue;
                var email = b.Email;
                var subject = "Friendly Reminder from Grace Chapel Book Library(pre)";
                var message = $"Hello {b.FullName}, <br /> <br />The return date of the book <b>{b.Book.Title}</b> is coming in next 5 days . <br />You can return the book within that time or renew it.<br />To renew the book please login using your Username and Password, and click on the extend button from the profile page.<br /> <br />Thank You <br /> Grace Chapel Library Admin";
                _emailService.SendEmail(email, subject, message);
            }
            );
        }
    }
}
