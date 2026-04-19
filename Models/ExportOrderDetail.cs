namespace MySellerApp.Models
{
    public class ExportOrderDetail
    {
        public int Id { get; set; }
        public int ExportOrderId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}