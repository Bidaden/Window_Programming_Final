using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface ISupplierRepository
    {
        List<Supplier> GetAll();
        int Add(Supplier supplier); // Trả về ID mới
        void Delete(int id);
    }
}