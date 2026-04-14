using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface IProductRepository
    {
        List<Product> GetAll();
        List<Product> SearchByName(string keyword);
    }
}
