using System.ComponentModel.DataAnnotations;
using System.Security.Principal;

namespace LabelSite.Models
{
    public class SalesData
    {
        [Key]
        public int SalesDataId { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public int SalesCount { get; set; }
        public int SalesGoal { get; set; }
        public decimal SalesPercentage { get; set; }
        public DateTime MonthStart { get; set; }
        public DateTime MonthEnd { get; set; }

        public ICollection<SalesProduct> SalesProducts { get; set; } // Новая связь
    }

}
