using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class SaleOrderRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public int CreateOrder(SaleOrder order,
                               List<SaleOrderDetail> details)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo SaleOrder
                        var cmdOrder = new SqlCommand(
                            "INSERT INTO SaleOrders " +
                            "(CustomerName, CustomerPhone, TotalAmount) " +
                            "VALUES (@name, @phone, @total); " +
                            "SELECT SCOPE_IDENTITY();", con, tran);
                        cmdOrder.Parameters.AddWithValue("@name", order.CustomerName);
                        cmdOrder.Parameters.AddWithValue("@phone", order.CustomerPhone);
                        cmdOrder.Parameters.AddWithValue("@total", order.TotalAmount);
                        int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                        foreach (var d in details)
                        {
                            // 2. Tạo SaleOrderDetail
                            var cmdDetail = new SqlCommand(
                                "INSERT INTO SaleOrderDetails " +
                                "(SaleOrderId, ProductId, Quantity, UnitPrice) " +
                                "VALUES (@oid, @pid, @qty, @price)", con, tran);
                            cmdDetail.Parameters.AddWithValue("@oid", orderId);
                            cmdDetail.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdDetail.Parameters.AddWithValue("@qty", d.Quantity);
                            cmdDetail.Parameters.AddWithValue("@price", d.UnitPrice);
                            cmdDetail.ExecuteNonQuery();

                            // 3. Trừ stock trong Products
                            var cmdStock = new SqlCommand(
                                "UPDATE Products " +
                                "SET Quantity = Quantity - @qty " +
                                "WHERE Id = @pid", con, tran);
                            cmdStock.Parameters.AddWithValue("@qty", d.Quantity);
                            cmdStock.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdStock.ExecuteNonQuery();

                            // 4. Lấy Quantity sau khi trừ
                            var cmdAfter = new SqlCommand(
                                "SELECT Quantity FROM Products WHERE Id = @pid",
                                con, tran);
                            cmdAfter.Parameters.AddWithValue("@pid", d.ProductId);
                            int qtyAfter = Convert.ToInt32(cmdAfter.ExecuteScalar());

                            // 5. Ghi StockMovement
                            var cmdMove = new SqlCommand(
                                "INSERT INTO StockMovements " +
                                "(ProductId, UserId, Type, QuantityChanged, " +
                                "QuantityAfter, Note) " +
                                "VALUES (@pid, 1, 'EXPORT', @changed, " +
                                "@after, @note)", con, tran);
                            cmdMove.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdMove.Parameters.AddWithValue("@changed", -d.Quantity);
                            cmdMove.Parameters.AddWithValue("@after", qtyAfter);
                            cmdMove.Parameters.AddWithValue("@note",
                                "Bán hàng - Đơn #" + orderId);
                            cmdMove.ExecuteNonQuery();
                        }

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
            var list = new List<SaleOrder>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM SaleOrders ORDER BY OrderDate DESC", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SaleOrder
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerName = reader.GetString(reader.GetOrdinal("CustomerName")),
                            CustomerPhone = reader.GetString(reader.GetOrdinal("CustomerPhone")),
                            OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                            TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount")),
                            Status = reader.GetString(reader.GetOrdinal("Status"))
                        });
                    }
                }
            }
            return list;
        }

        public List<SaleOrderDetail> GetDetailsByOrderId(int orderId)
        {
            var list = new List<SaleOrderDetail>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT sd.*, p.Name AS ProductName, p.SKU " +
                    "FROM SaleOrderDetails sd " +
                    "JOIN Products p ON sd.ProductId = p.Id " +
                    "WHERE sd.SaleOrderId = @id", con);
                cmd.Parameters.AddWithValue("@id", orderId);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SaleOrderDetail
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            SaleOrderId = reader.GetInt32(reader.GetOrdinal("SaleOrderId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                            SKU = reader.GetString(reader.GetOrdinal("SKU")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
                        });
                    }
                }
            }
            return list;
        }
    }
}