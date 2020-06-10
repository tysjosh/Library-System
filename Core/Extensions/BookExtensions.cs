using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Extensions
{
    public static class BookExtensions
    {
        public static void Map(this Book dbBook, Book book)
        {
            dbBook.Title = book.Title;
            dbBook.AuthorName = book.AuthorName;
            dbBook.Isbn= book.Isbn;
            dbBook.Language= book.Language;
            dbBook.Description = book.Description;
            dbBook.NoOfCopiesActual = book.NoOfCopiesActual;
            dbBook.NoOfCopiesCurrent = book.NoOfCopiesCurrent;
            dbBook.PublicationYear = book.PublicationYear;
            dbBook.ThumbLink = book.ThumbLink;
            dbBook.CategoryId = book.CategoryId;
            dbBook.Location = book.Location;
        }
    }
}
