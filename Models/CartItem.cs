namespace MySellerApp.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public decimal SubTotal { get { return Quantity * UnitPrice; } }
    }
}