using System.Collections.Generic;
using System.Linq;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class ProductRepositoryEF : IProductRepository
    {
        public List<Product> GetAll()
        {
            using (var db = new AppDbContext())
            {
                var raw = db.Products.ToList();

                var cats = db.Categories.ToList();
                var sups = db.Suppliers.ToList();

                return raw.Select(p => new Product
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    MinStock = p.MinStock,
                    CategoryId = p.CategoryId,
                    SupplierId = p.SupplierId,
                    Category = cats.Where(c => c.Id == p.CategoryId)
                                     .Select(c => c.Name)
                                     .FirstOrDefault(),
                    Supplier = sups.Where(s => s.Id == p.SupplierId)
                                     .Select(s => s.Name)
                                     .FirstOrDefault()
                }).ToList();
            }
        }

        public List<Product> SearchByName(string keyword)
        {
            using (var db = new AppDbContext())
            {
                var raw = db.Products
                    .Where(p => p.Name.Contains(keyword) ||
                                p.SKU.Contains(keyword))
                    .ToList();

                var cats = db.Categories.ToList();
                var sups = db.Suppliers.ToList();

                return raw.Select(p => new Product
                {
                    Id = p.Id,
                    SKU = p.SKU,
                    Name = p.Name,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    MinStock = p.MinStock,
                    CategoryId = p.CategoryId,
                    SupplierId = p.SupplierId,
                    Category = cats.Where(c => c.Id == p.CategoryId)
                                     .Select(c => c.Name)
                                     .FirstOrDefault(),
                    Supplier = sups.Where(s => s.Id == p.SupplierId)
                                     .Select(s => s.Name)
                                     .FirstOrDefault()
                }).ToList();
            }
        }

        public void Add(Product p)
        {
            using (var db = new AppDbContext())
            {
                db.Products.Add(p);
                db.SaveChanges();
            }
        }

        public void UpdateStock(int productId, int change)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products.Find(productId);
                if (product != null)
                {
                    product.Quantity += change;
                    db.SaveChanges();
                }
            }
        }

        public void LogStockMovement(int productId, int userId,
            string type, int changed, int after, string note)
        {
            using (var db = new AppDbContext())
            {
                db.StockMovements.Add(new StockMovement
                {
                    ProductId = productId,
                    UserId = userId,
                    Type = type,
                    QuantityChanged = changed,
                    QuantityAfter = after,
                    Note = note
                });
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var product = db.Products.Find(id);
                if (product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
            }
        }
    }
}