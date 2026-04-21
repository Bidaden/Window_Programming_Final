using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface IExportOrderRepository
    {
        List<ExportOrder> GetAll();
        void Create(int userId, string reason, string note,
                   List<ExportOrderDetail> details, IProductRepository productRepo);
    }
}