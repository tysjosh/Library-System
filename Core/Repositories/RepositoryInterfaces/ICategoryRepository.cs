using System.Collections.Generic;
using System.Threading.Tasks;
using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Repositories.RepositoryInterfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<IEnumerable<Category>> GetAllCategoriesWithoutRelationAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task CreateCategoryAsync(Category category);
        Task UpdateCategoryAsync(Category dbCategory, Category category);
        Task DeleteCategoryAsync(Category category);
        bool CategoryExistsByName(string catName);
    }
}