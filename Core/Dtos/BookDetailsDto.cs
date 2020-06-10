using System;
using System.Collections.Generic;

namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class BookDetailsDto
    {
        public BookDetailsDto()
        {
            Borrowers = new List<BorrowerDto>();
        }
        public int Id { get; set; }
        
        public string Title { get; set; }
        
        public string Isbn { get; set; }

        public string Description { get; set; }
        public DateTime PublicationYear { get; set; }

        
        public string AuthorName { get; set; }

        public string ThumbLink { get; set; }

        public string Language { get; set; }

        public string Location { get; set; }

        public int NoOfCopiesActual { get; set; }

        public int NoOfCopiesCurrent { get; set; }
        
        public CategoryDto Category { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<BorrowerDto> Borrowers { get; set; }
    }
}
