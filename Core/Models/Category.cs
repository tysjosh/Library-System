using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GraceChapelLibraryWebApp.Core.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Category name required")]
        public string Name { get; set; }
        public int ParentId { get; set; }
        public ICollection<Book> Books { get; set; }
        public ICollection<Category> Children { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
