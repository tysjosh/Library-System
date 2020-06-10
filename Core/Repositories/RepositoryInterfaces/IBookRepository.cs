using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync(string searchString, int catId, string language, string location);
        Task<IEnumerable<BookListWithoutDetailsDto>> GetAllBooksAsyncByCategory(int category);
        
        Task<Book> GetBookByIdAsync(int bookId);
        Task<BookDetailsDto> GetBookWithDetailsAsync(int bookId);
        Task<BookDetailsDto> GetBookDetailsAsyncByIsbn(string isbn);
        
        Task<IEnumerable<BookDetailsDto>> GetAllBookDetailsAsync(string searchString);
        Task<IEnumerable<BookDetailsDto>> GetAllBookDetailsAsyncByCategory(int category);
        
        Task CreateBookAsync(Book book);
        Task UpdateBookAsync(Book dbBook, Book book);
        Task DeleteBookAsync(Book book);
        Task UpdateCurrentAmount(int bookId, int currentBookAmount); 
        bool BookExists(int id);
        bool BookExistsByIsbn( string isbn);
    }
}