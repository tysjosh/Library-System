using AutoMapper;
using GraceChapelLibraryWebApp.Core.Dtos;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Extensions
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<BookUser, ApplicationUser>();
            CreateMap<Book, BookDto>();
            CreateMap<Borrower, BorrowerDto>();
            CreateMap<Category, CategoryDto>();
            CreateMap<Book, BookDetailsDto>();
            CreateMap<Borrower, BorrowerDetailsDto>();
        }
    }
}
