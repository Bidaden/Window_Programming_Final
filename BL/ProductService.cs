using System;
using System.Collections.Generic;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.BL
{
    public class ProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public List<Product> GetAllProducts()
        {
            return _repo.GetAll();
        }

        public List<Product> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                throw new Exception("Vui lòng nhập từ khóa tìm kiếm!");
            return _repo.SearchByName(keyword);
        }
    }
}