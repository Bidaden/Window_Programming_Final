using System;

namespace MySellerApp.Models
{
    public class ImportOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public DateTime ImportDate { get; set; }
        public string Note { get; set; }
    }
}