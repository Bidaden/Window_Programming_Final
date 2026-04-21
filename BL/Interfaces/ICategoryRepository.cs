using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        void Add(Category category);
        void Delete(int id);
    }
}