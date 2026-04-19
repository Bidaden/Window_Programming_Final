using System;
using System.Collections.Generic;
using System.Linq;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class SaleOrderRepositoryEF
    {
        public int CreateOrder(SaleOrder order,
            List<SaleOrderDetail> details)
        {
            using (var db = new AppDbContext())
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo SaleOrder
                        db.SaleOrders.Add(order);
                        db.SaveChanges();
                        int orderId = order.Id;

                        foreach (var d in details)
                        {
                            d.SaleOrderId = orderId;

                            // 2. Tạo SaleOrderDetail
                            db.SaleOrderDetails.Add(d);

                            // 3. Trừ stock
                            var product = db.Products.Find(d.ProductId);
                            if (product != null)
                            {
                                product.Quantity -= d.Quantity;
                                int qtyAfter = product.Quantity;

                                // 4. Ghi StockMovement
                                db.StockMovements.Add(new StockMovement
                                {
                                    ProductId = d.ProductId,
                                    UserId = 1,
                                    Type = "EXPORT",
                                    QuantityChanged = -d.Quantity,
                                    QuantityAfter = qtyAfter,
                                    Note = "Ban hang - Don #" + orderId
                                });
                            }
                        }

                        db.SaveChanges();
                        tran.Commit();
                        return orderId;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<SaleOrder> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.SaleOrders
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
            }
        }

        public List<SaleOrderDetail> GetDetailsByOrderId(int orderId)
        {
            using (var db = new AppDbContext())
            {
                return db.SaleOrderDetails
                    .Where(d => d.SaleOrderId == orderId)
                    .Select(d => new SaleOrderDetail
                    {
                        Id = d.Id,
                        SaleOrderId = d.SaleOrderId,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPrice,
                        ProductName = db.Products
                                        .Where(p => p.Id == d.ProductId)
                                        .Select(p => p.Name)
                                        .FirstOrDefault(),
                        SKU = db.Products
                                        .Where(p => p.Id == d.ProductId)
                                        .Select(p => p.SKU)
                                        .FirstOrDefault()
                    })
                    .ToList();
            }
        }
    }
}