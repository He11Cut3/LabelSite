using System.ComponentModel.DataAnnotations;

namespace LabelSite.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; }
        public ICollection<SalesProduct> SalesProducts { get; set; }
    }
}
