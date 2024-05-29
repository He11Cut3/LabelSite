namespace LabelSite.Models
{
    public class SalesProduct
    {
        public int SalesDataId { get; set; }
        public SalesData SalesData { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; } // Количество товара в продаже
    }
}
