using System;

namespace MySellerApp.Models
{
    public class ExportOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ExportDate { get; set; }
        public string Reason { get; set; }
        public string Note { get; set; }
    }
}