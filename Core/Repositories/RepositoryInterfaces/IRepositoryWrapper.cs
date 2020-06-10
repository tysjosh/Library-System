namespace GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces
{
    public interface IRepositoryWrapper
    {
        IBookRepository Book { get; }
        ICategoryRepository Category { get; }
        IBorrowerRepository Borrower { get; }

        IUserRepository User { get; }

    }
}
