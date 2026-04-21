using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface IImportOrderRepository
    {
        List<ImportOrder> GetAll();
        void Create(int supplierId, int userId, string note,
                   List<ImportOrderDetail> details, IProductRepository productRepo);
    }
}