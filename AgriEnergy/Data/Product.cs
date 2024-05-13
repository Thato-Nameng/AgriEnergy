using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergy.Data
{
    [Table("Product")]
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string? ProductName { get; set; }
        [Required]
        public string? ProductDescription { get; set; }

        [Required]
        public double ProductCosts { get; set; }
    }
}
