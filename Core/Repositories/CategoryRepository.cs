using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Services;
using GraceChapelLibraryWebApp.Core.Extensions;
using GraceChapelLibraryWebApp.Core.Models;
using GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces;

namespace GraceChapelLibraryWebApp.Core.Repositories
{
    public class CategoryRepository: RepositoryBase<Category>, ICategoryRepository

    {
        private BookLibraryContext _bookLibraryContext;
        private IEmailService _emailService;
         public CategoryRepository(BookLibraryContext repositoryContext, IEmailService emailService)
        : base(repositoryContext, emailService)
        {
            _bookLibraryContext = repositoryContext;
            _emailService = emailService;
        }
        public async Task CreateCategoryAsync(Category category)
        {
            Create(category);
            await SaveAsync();
        }

        public async Task DeleteCategoryAsync(Category category)
        {
            Delete(category);
            await SaveAsync();

        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {

            //var BookCategories = await FindAllAsync();
            //return BookCategories.OrderBy(x => x.Name);
            var categories = await FindAllAsync();
            List<Category> hierarchy = new List<Category>();
            hierarchy = categories.ToList()
                            .Where(c => c.ParentId == 0)
                            .Select(c => new Category()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                ParentId = c.ParentId,
                                Children = _GetChildren(categories.ToList(), c.Id)
                            })
                            .ToList();
            IEnumerable<Category> returnCat = hierarchy as IEnumerable<Category>;
            return returnCat.OrderBy(x => x.Name);

        }

        private List<Category> _GetChildren(List<Category> categories, int parentId)
        {

            return categories
                    .Where(c => c.ParentId == parentId)
                    .Select(c => new Category
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ParentId = c.ParentId,
                        Children = _GetChildren(categories.ToList(), c.Id)
                    })
                    .ToList();
        }

        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            var Category = await FindByConditionAync(o => o.Id.Equals(categoryId));
            return Category.DefaultIfEmpty(new Category())
                    .FirstOrDefault();
        }

        public async Task UpdateCategoryAsync(Category dbCategory, Category Category)
        {
            dbCategory.Map(Category);
            Update(dbCategory);
            await SaveAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesWithoutRelationAsync()
        {
            var categories = await FindAllAsync();
            return categories.OrderBy(x => x.Name);
        }

        public bool CategoryExistsByName(string catName)
        {
            return _bookLibraryContext.BookCategories.Any(c => c.Name == catName);
        }
    }
}
