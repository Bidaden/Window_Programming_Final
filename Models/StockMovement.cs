using System;

namespace MySellerApp.Models
{
    public class StockMovement
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Type { get; set; }
        public int QuantityChanged { get; set; }
        public int QuantityAfter { get; set; }
        public DateTime MovedAt { get; set; }
        public string Note { get; set; }
    }
}