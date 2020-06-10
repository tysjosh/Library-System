namespace GraceChapelLibraryWebApp.Core.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public string Language { get; set; }
        public string Location { get; set; }
        public int NoOfCopiesActual { get; set; }
        public int NoOfCopiesCurrent { get; set; }


    }
}