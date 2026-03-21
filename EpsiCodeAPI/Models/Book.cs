using System.ComponentModel.DataAnnotations;

namespace EpsiCodeAPI.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public int NumberOfCopies { get; set; }
        public decimal Price { get; set; }
        public int Number { get; set; }
        public string Title { get; set; } = string.Empty;
        public string OriginalTitle { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Pages { get; set; }
        public string Cover { get; set; } = string.Empty;
        public int Index { get; set; }
    }
}
