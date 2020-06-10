using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraceChapelLibraryWebApp.Core.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Book Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Book must have ISBN")]
        public string Isbn { get; set; }
        public string Description { get; set; }
        public DateTime PublicationYear { get; set; }

        [MaxLength(100, ErrorMessage = "AuthorName cannot be longer than 100 characters")]
        public string AuthorName { get; set; }

        public string ThumbLink { get; set; }

        public string Language { get; set; }
        public string Location { get; set; }

        [Required(ErrorMessage = "Number of copies must be 1 or more")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than 1")]
        public int NoOfCopiesActual { get; set; }

        public int NoOfCopiesCurrent { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required(ErrorMessage = "Book must have category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a category value bigger than 1")]
        public int CategoryId { get; set; }

        public ICollection<Borrower> Borrowers { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
    }
}
