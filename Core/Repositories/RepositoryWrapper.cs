using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;
using GraceChapelLibraryWebApp.Core.Templates;
using Microsoft.AspNetCore.Http;
using AutoMapper;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly BookLibraryContext _libraryContext;
        private readonly IEmailService _emailService;
        private BookRepository _book;
        private readonly IMapper _mapper;
        private BorrowerRepository _borrower;
        private CategoryRepository _category;
        private UserRepository _user;
        private readonly IEmailTemplate _emailTemplate;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RepositoryWrapper(BookLibraryContext bookLibraryContext, IMapper mapper, IEmailService emailService, IEmailTemplate emailTemplate, IHttpContextAccessor httpContextAccessor)
        {
            _libraryContext = bookLibraryContext;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        public IBookRepository Book {
            get {
                if (_book == null) {
                    _book = new BookRepository(_libraryContext, _emailService, _mapper);
                }
                return _book;
            }
        }

        public IBorrowerRepository Borrower
        {
            get
            {
                if (_borrower == null)
                {
                    _borrower = new BorrowerRepository(_libraryContext, _emailService, _emailTemplate, _mapper);
                }
                return _borrower;
            }
        }

        public ICategoryRepository Category
        {
            get
            {
                if (_category == null)
                {
                    _category = new CategoryRepository(_libraryContext, _emailService);
                }
                return _category;
            }
        }
        public IUserRepository User
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserRepository(_libraryContext, _emailService, _httpContextAccessor, _mapper);
                }
                return _user;
            }
        }
    }
}
