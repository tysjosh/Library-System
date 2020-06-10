using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Controllers
{

    // GET: api/Books

    /// <summary>
    /// Get all Books( Only list ).
    /// </summary>
    ///
    [Route("api/[controller]")]
    [ApiController]

    public class BooksController : AppController
    {
        private readonly IRepositoryWrapper _repoWrapper;
        // Book Controller 
        /// <summary>
        /// Book Controller .
        /// </summary>
        ///
        public BooksController(IRepositoryWrapper repoWrapper)
        {
            _repoWrapper = repoWrapper;
        }

        // GET: api/Books

        /// <summary>
        /// Get all Books( Only list ).
        /// </summary>
        ///

        [HttpGet("params")]
        public async Task<IEnumerable<Book>> GetBooks(string key = "all", int catId = 0, string language = "all", string location = "all")
        {
            try
            {
                return await _repoWrapper.Book.GetAllBooksAsync(key, catId, language, location);

            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Books/book-all-details

        /// <summary>
        /// Get all Books( Detail lists ).
        /// </summary>
        ///

        [HttpGet("book-all-details/{key}")]
        public async Task<IEnumerable<BookDetailsDto>> GetAllBookDetails(string key = "all")
        {
            try
            {
                return await _repoWrapper.Book.GetAllBookDetailsAsync(key);

            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Books/book-details-by-category/3

        /// <summary>
        /// Get all Books By Category( Detail lists ).
        /// </summary>
        ///

        [HttpGet("book-details-by-category/{category}")]
        public async Task<IEnumerable<BookDetailsDto>> GetAllBookDetailsByCategory(int category)
        {
            try
            {
                return await _repoWrapper.Book.GetAllBookDetailsAsyncByCategory(category);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: api/Books/book-details-by-isbn/3465464646

        /// <summary>
        /// Get Book By ISBN( With borrowers).
        /// </summary>
        ///

        [HttpGet("book-details-by-isbn/{isbn}")]
        public async Task<ActionResult<BookDetailsDto>> GetBookDetailsByIsbn([FromRoute] string isbn)
        {
            try
            {
                var book = await _repoWrapper.Book.GetBookDetailsAsyncByIsbn(isbn);
                if (book == null) {
                    return NotFound("Book not found");
                }
                return book;
            }
            catch (Exception)
            {
                throw;
            }
        }


        // GET: api/Books

        /// <summary>
        /// Get all Books by category( Only list ).
        /// </summary>
        ///

        [HttpGet("book-lists-by-category/{category}")]
        public async Task<IEnumerable<BookListWithoutDetailsDto>> GetAllBookListsByCategory(int category)
        {
            try
            {
                return await _repoWrapper.Book.GetAllBooksAsyncByCategory(category);

            }
            catch (Exception)
            {
                throw;
            }
        }




        //Get: api/Book/1
        /// <summary>
        /// Book Information without details(category, borrower information).
        /// </summary>
        ///
        [HttpGet("{id}")]

        public async Task<ActionResult<Book>> GetBook(int id) {
            var book = await _repoWrapper.Book.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        //Get: api/book-details/7
        /// <summary>
        /// Single Book information with details
        /// </summary>
        ///
        [HttpGet("book-details/{id}")]
       
        public async Task<ActionResult<BookDetailsDto>> GetBookAsync([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var results = await _repoWrapper.Book.GetBookWithDetailsAsync(id);

            if (results == null)
            {
                return NotFound();
            }

            return results ;
        }
        // POST: api/Books
        /// <summary>
        /// Add a book.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Book
        ///     {
        ///        "Title": "Sample Book Title",
        ///        "AuthorName": "John Doe",
        ///        "CategoryId": 100,
        ///        "No_Of_Copies_Actual": 20,
        ///        "Isbn" : 4582134,
        ///        "Language": "Eng",
        ///        "Publication_Year": "2018-10-01"
        ///        
        ///     }
        ///
        /// </remarks>
        /// 

        [HttpPost, Authorize(Roles = "Admin")]
        //[HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var isExists =  _repoWrapper.Book.BookExistsByIsbn(book.Isbn);
            if (isExists)
            {
                return Conflict("Already exists with the same isbn");
            }
            if (ModelState.IsValid) {
                await _repoWrapper.Book.CreateBookAsync(book);
                return CreatedAtAction("GetBook", new { id = book.Id }, book);
            }
            return NoContent();
        }

        // PUT: api/Books/5
        /// <summary>
        /// Update Book Information
        /// </summary>
        ///
        [HttpPut("{id}"), Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id)
            {
                return BadRequest();
            }

            try
            {
                var dbBook = await _repoWrapper.Book.GetBookByIdAsync(id);
                await _repoWrapper.Book.UpdateBookAsync(dbBook, book);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_repoWrapper.Book.BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Books/5

        /// <summary>
        /// Delete Book.
        /// </summary>
        /// 
        [HttpDelete("{id}"), Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _repoWrapper.Book.GetBookByIdAsync(id);
                if (book == null)
                {
                    return NotFound();
                }

                await _repoWrapper.Book.DeleteBookAsync(book);

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}