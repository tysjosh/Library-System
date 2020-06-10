using GraceChapelLibraryWebApp.Core.Models;

namespace GraceChapelLibraryWebApp.Core.Extensions
{
    public static class CategoryExtensions
    {
        public static void Map(this Category dbCategory, Category category)
        {
            dbCategory.Name = category.Name;
            dbCategory.ParentId = category.ParentId;
        }
    }
}
