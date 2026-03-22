using System.Text.Json.Serialization;

namespace EpsiCodeWeb.Models
{
    public class BookViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Cover { get; set; } = string.Empty;
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
        public int NumberOfCopies { get; set; }
    }
}
