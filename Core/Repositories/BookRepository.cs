using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Extensions;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public class BookRepository : RepositoryBase<Book>, IBookRepository

    {
        private readonly BookLibraryContext _context;
        private readonly IMapper _mapper;
        private IEmailService _emailService;
        private List<Book> _bookEntity;

        public BookRepository(BookLibraryContext repositoryContext, IEmailService emailService, IMapper mapper)
        : base(repositoryContext, emailService)
        {
            _mapper = mapper;
            _context = repositoryContext;
            _emailService = emailService;
        }

        public async Task CreateBookAsync(Book book)
        {
            Create(book);
            await SaveAsync();
        }
        public async Task DeleteBookAsync(Book book)
        {
            Delete(book);
            await SaveAsync();

        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync(string searchStr, int catId, string languageStr, string locationStr)
        {
            var Books = await FindAllAsync();
            var searchKey = searchStr.ToLower().Trim();
            if (!String.IsNullOrEmpty(searchKey) && searchKey != "all")
            {
                Books = Books.Where(
                    b => b.Title.ToLower().Contains(searchKey)
                        || b.Description.ToLower().Contains(searchKey)
                        || b.AuthorName.ToLower().Contains(searchKey)
                        || b.Language.ToLower().Contains(searchKey)
                    );
            }
            if (catId > 0)
            {
                Books = Books.Where(
                    b => b.CategoryId == catId
                    );
            }
            var languageKey = languageStr.ToLower().Trim();
            if (!String.IsNullOrEmpty(languageKey) && languageKey != "all")
            {
                Books = Books.Where(
                    b => b.Language == languageKey
                    );
            }
            var locationKey = locationStr.Trim();
            
            if (!String.IsNullOrEmpty(locationKey) && locationKey != "all")
            {
                Books = Books.Where(
                    b => b.Location == locationKey
                    );
            }
            return Books.OrderBy(x => x.Title);
        }

        public async Task<Book> GetBookByIdAsync(int bookId)
        {
            var Book = await FindByConditionAync(o => o.Id.Equals(bookId));
            return Book.FirstOrDefault();
        }

        public async Task UpdateBookAsync(Book dbBook, Book book)
        {
            dbBook.Map(book);
            Update(dbBook);
            await SaveAsync();
        }

        public bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        public async Task<BookDetailsDto> GetBookWithDetailsAsync(int bookId)
        {
            var bookEntity = await _context.Books.Include(b => b.Borrowers)
                .Include(b => b.Category).Where(b => b.Id == bookId).FirstOrDefaultAsync();
            var results = _mapper.Map<BookDetailsDto>(bookEntity);
            return results;
        }

        public async Task<IEnumerable<BookDetailsDto>> GetAllBookDetailsAsync(string searchString)
        {
            var searchKey = searchString.ToLower();

            if (!String.IsNullOrEmpty(searchKey) && searchKey != "all")
            {
                _bookEntity = await _context.Books.Include(b => b.Borrowers).Include(b => b.Category).Where(
                 b => b.Title.ToLower().Contains(searchKey)
                        || b.Description.ToLower().Contains(searchKey)
                        || b.AuthorName.ToLower().Contains(searchKey)
                        || b.Language.ToLower().Contains(searchKey)
                ).ToListAsync();
            }
            else
            {
                _bookEntity = await _context.Books.Include(b => b.Borrowers).Include(b => b.Category).ToListAsync();
            }
            var results = _mapper.Map<IEnumerable<BookDetailsDto>>(_bookEntity);
            return results;
        }

        public async Task<IEnumerable<BookDetailsDto>> GetAllBookDetailsAsyncByCategory(int category)
        {
            _bookEntity = await _context.Books.Include(b => b.Borrowers).Include(b => b.Category).Where(
                 b => b.CategoryId == category).ToListAsync();
            var results = _mapper.Map<IEnumerable<BookDetailsDto>>(_bookEntity);
            return results;
        }

        public async Task<IEnumerable<BookListWithoutDetailsDto>> GetAllBooksAsyncByCategory(int category)
        {
            _bookEntity = await _context.Books.Include(b => b.Category).Where(
               b => b.CategoryId == category).ToListAsync();
            var results = _mapper.Map<IEnumerable<BookListWithoutDetailsDto>>(_bookEntity);
            return results;
        }

        public bool BookExistsByIsbn(string isbn)
        {
            return _context.Books.Any(e => e.Isbn == isbn);
        }

        public async Task<BookDetailsDto> GetBookDetailsAsyncByIsbn(string isbn)
        {
            var bookEntity = await _context.Books.Include(b => b.Borrowers)
                .Include(b => b.Category).Where(b => b.Isbn == isbn).FirstOrDefaultAsync();
            var results = _mapper.Map<BookDetailsDto>(bookEntity);
            return results;
        }

        public async Task UpdateCurrentAmount(int bookId, int currentBookAmount)
        {
            var book =  new Book { Id = bookId };
            book.NoOfCopiesCurrent = currentBookAmount;
            _context.Entry(book).Property("No_Of_Copies_Current").IsModified = true;
            _context.SaveChanges();
        }
    }
}
