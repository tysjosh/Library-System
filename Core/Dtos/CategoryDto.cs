using System;

namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int ParentId { get; set; }
    }
}
