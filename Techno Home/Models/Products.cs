using System.ComponentModel.DataAnnotations;

namespace Techno_Home.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        public string? Genre { get; set; }
        // [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string? ImageFileName { get; set; }
    }
    
    
}
