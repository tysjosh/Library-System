using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Enumerations;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using GraceChapelLibraryWebApp.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GraceChapelLibraryWebApp.Controllers
{

    // Controller: Borrowers

    /// <summary>
    /// Borrower Controller.
    /// </summary>
    ///
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowersController : AppController
    {
        private readonly IRepositoryWrapper _repoWrapper;
        private readonly IEmailService _emailService;
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        // Controller: Borrowers

        /// <summary>
        /// Borrower Controller.
        /// </summary>
        ///
        public BorrowersController(IRepositoryWrapper repoWrapper, IEmailService emailService,
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _repoWrapper = repoWrapper;
            _emailService = emailService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Borrowers

        /// <summary>
        /// Get all borrowers.
        /// </summary>
        ///

        [HttpGet("params")]
        public async Task<IEnumerable<BorrowerDetailsDto>> GetBorrowers(string statusFilter = "all", string timeRangeFilter = "all")
        {
            try
            {
                return await _repoWrapper.Borrower.GetAllBorrowersAsync(statusFilter, timeRangeFilter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Get: api/Borrowers/1

        /// <summary>
        /// Get borrower by id.
        /// </summary>
        ///
        [HttpGet("{id}")]

        public async Task<ActionResult<Borrower>> GetBorrower(int id)
        {
            var borrower = await _repoWrapper.Borrower.GetBorrowerByIdAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }
            return borrower;
        }
        //Get: api/Borrowers/23641034/1


        //[HttpGet("find_borrower/{empId}/{bookId}")]

        //public async Task<ActionResult<Borrower>> GetBorrowerByEmployeeIdAndBookId(int empId, int bookId)
        //{
        //    var borrower = await _repoWrapper.Borrower.GetBorrowerByEmployeeIdAndBookIdIdAsync(empId, bookId);
        //    if (borrower == null)
        //    {
        //        return NotFound("The borrower not found");
        //    }
        //    return borrower;
        //}
        //Get: api/Borrowers/23641034/1

        /// <summary>
        /// Get borrower by signum
        /// </summary>
        ///
        //[HttpGet("find_by_signum/{signum}")]

        //public async Task<IEnumerable<BorrowerDetailsDto>> GetBorrowerByEmployeeIdAndBookId(string signum)
        //{
        //    try
        //    {
        //        return await _repoWrapper.Borrower.GetBorrowerBySignumAsync(signum);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpPost]
        public async Task<ActionResult<Borrower>> PostBorrower([FromBody] Borrower borrower)
        {
            borrower.BorrowedFromDate = DateTime.Now;
            borrower.BorrowedToDate = DateTime.Now.AddMonths(1);
            var userInfo = await _repoWrapper.User.GetUserByIdAsync(borrower.Id);

            borrower.Email = userInfo.Email;

            var existingBorrower = _repoWrapper.Borrower.IsExists(borrower);
            if (existingBorrower)
            {
                return Conflict("Already exists in borrowed condition");
            }
            try
            {
                await _repoWrapper.Borrower.CreateBorrowerAsync(borrower);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
            // update the book and change the current available 

            return CreatedAtAction("GetBorrower", new { id = borrower.Id }, borrower);
        }

        // DELETE: api/Borrowers/5

        /// <summary>
        /// Delete borrower.
        /// </summary>
        /// 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBorrower(int id)
        {
            try
            {
                var borrower = await _repoWrapper.Borrower.GetBorrowerByIdAsync(id);
                if (borrower == null)
                {
                    return NotFound();
                }

                await _repoWrapper.Borrower.DeleteBorrowerAsync(borrower);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }


        // PUT: api/Borrowers/Extend/5

        /// <summary>
        /// Extend the return date.
        /// </summary>
        /// 
        [HttpPut("Extend/{id}"), Authorize(Roles = "Admin, BookUser")]
        public async Task<IActionResult> PutExtendBook(int id)
        {
            try
            {
                var dbBorrower = await _repoWrapper.Borrower.GetBorrowerByIdAsync(id);
                if (dbBorrower == null)
                {
                    return NotFound();
                }
                var currentUserRole = _repoWrapper.User.FindCurrentUserRole();
                if (currentUserRole == "BookUser")
                {
                    //check if the book already extended earlier or not 
                    if (dbBorrower.ExtendCount >= 1)
                    {
                        return BadRequest("You are not allowed to extend book multiple times. Please contact with admin");
                    }
                }
                await _repoWrapper.Borrower.ExtendBookAsync(dbBorrower);

                return Ok(new { status = "success", message = "Successfully extended the borrowed time" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Borrowers/Return/5

        /// <summary>
        /// Return a book.
        /// </summary>
        /// 
        [HttpPut("Return/{borrower_id}")]
        public async Task<IActionResult> PutReturnBook(int borrowerId)
        {
            try
            {
                var dbBorrower = await _repoWrapper.Borrower.GetBorrowerByIdAsync(borrowerId);
                if (dbBorrower == null)
                {
                    return NotFound("Borrower not found");
                }

                // if not borrowed condition return not borrowed 
                if (dbBorrower.BorrowStatus == BorrowStatus.Returned)
                {
                    var result = new ObjectResult("The book in not in borrowed condition");
                    result.StatusCode = 406;
                    return result;
                    //return StatusCode(404, "The book in not in borrowed condition");
                }

                await _repoWrapper.Borrower.ReturnBookAsync(dbBorrower);
                // update the current book amount 
                var dbBook = await _repoWrapper.Book.GetBookByIdAsync(dbBorrower.BookId);
                var book = dbBook;
                book.NoOfCopiesCurrent = dbBook.NoOfCopiesCurrent + 1;
                await _repoWrapper.Book.UpdateBookAsync(dbBook, book);

                return Ok(new { status = "success", message = "Successfully returned the book" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/Borrowers/send-reminder/5

        /// <summary>
        /// Send Email Reminder.
        /// </summary>
        /// 
        [HttpGet("send-reminder/{borrower_id}")]


        public async Task<IActionResult> SendEmail(int borrowerId)
        {
            var borrower = await _repoWrapper.Borrower.GetBorrowerByIdAsync(borrowerId);
            var bookInfo = await _repoWrapper.Book.GetBookByIdAsync(borrower.BookId);
            var email = borrower.Email;
            var subject = "Friendly Reminder from Grace Chapel Library";
            var message = $"Hello {borrower.FullName}, <br /> <br />The return date of the book <b>{bookInfo.Title}</b> has expired. <br />Please renew the borrowed book or return it. " +
                          $"To renew the book please login using your Username and Password and click on the extend button from the profile page. <br /> <br />Thank You <br /> Grace Chapel Library Admin";
            try
            {
                await _emailService.SendEmail(email, subject, message);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpGet("decrease-book-amount/{bookId}")]
        public async Task<IActionResult> UpdateBookAmount(int bookId)
        {
            var dbBook = await _repoWrapper.Book.GetBookByIdAsync(bookId);
            var book = dbBook;
            if (dbBook.NoOfCopiesCurrent == 0)
            {
                return StatusCode(406, "No Book Available");
            }
            var currentBookAmount = book.NoOfCopiesCurrent = (dbBook.NoOfCopiesCurrent > 0 && dbBook.NoOfCopiesCurrent 
                                                                <= dbBook.NoOfCopiesActual) ? dbBook.NoOfCopiesCurrent - 1 : dbBook.NoOfCopiesActual - 1;
            try
            {
                await _repoWrapper.Book.UpdateCurrentAmount(bookId, currentBookAmount);
            }
            catch (Exception)
            {

                return StatusCode(500, "Internal server error");
            }
            return Ok(new { status = "success", message = "Successfully decreased the amount of the book" });

        }

    }
}