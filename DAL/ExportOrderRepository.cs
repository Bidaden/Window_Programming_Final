using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class ExportOrderRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<ExportOrder> GetAll()
        {
            var list = new List<ExportOrder>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT eo.Id, eo.ExportDate, eo.Reason, eo.Note, " +
                    "u.Name AS UserName " +
                    "FROM ExportOrders eo " +
                    "JOIN Users u ON eo.UserId = u.Id " +
                    "ORDER BY eo.ExportDate DESC", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ExportOrder
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            ExportDate = reader.GetDateTime(reader.GetOrdinal("ExportDate")),
                            Reason = reader.IsDBNull(reader.GetOrdinal("Reason"))
                                         ? "" : reader.GetString(reader.GetOrdinal("Reason")),
                            Note = reader.IsDBNull(reader.GetOrdinal("Note"))
                                         ? "" : reader.GetString(reader.GetOrdinal("Note"))
                        });
                    }
                }
            }
            return list;
        }

        public void Create(int userId, string reason, string note,
            List<ExportOrderDetail> details,
            ProductRepository productRepo)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                using (var tran = con.BeginTransaction())
                {
                    try
                    {
                        // 1. Tạo ExportOrder
                        var cmdOrder = new SqlCommand(
                            "INSERT INTO ExportOrders " +
                            "(UserId, Reason, Note) " +
                            "VALUES (@uid, @reason, @note); " +
                            "SELECT SCOPE_IDENTITY();", con, tran);
                        cmdOrder.Parameters.AddWithValue("@uid", userId);
                        cmdOrder.Parameters.AddWithValue("@reason", reason ?? "");
                        cmdOrder.Parameters.AddWithValue("@note", note ?? "");
                        int orderId = Convert.ToInt32(cmdOrder.ExecuteScalar());

                        foreach (var d in details)
                        {
                            // 2. Tạo ExportOrderDetail
                            var cmdDetail = new SqlCommand(
                                "INSERT INTO ExportOrderDetails " +
                                "(ExportOrderId, ProductId, Quantity) " +
                                "VALUES (@oid, @pid, @qty)", con, tran);
                            cmdDetail.Parameters.AddWithValue("@oid", orderId);
                            cmdDetail.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdDetail.Parameters.AddWithValue("@qty", d.Quantity);
                            cmdDetail.ExecuteNonQuery();

                            // 3. Giảm stock trong Products
                            var cmdStock = new SqlCommand(
                                "UPDATE Products " +
                                "SET Quantity = Quantity - @qty " +
                                "WHERE Id = @pid", con, tran);
                            cmdStock.Parameters.AddWithValue("@qty", d.Quantity);
                            cmdStock.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdStock.ExecuteNonQuery();

                            // 4. Lấy stock sau khi giảm
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
                                "VALUES (@pid, @uid, 'EXPORT', @changed, " +
                                "@after, @note)", con, tran);
                            cmdMove.Parameters.AddWithValue("@pid", d.ProductId);
                            cmdMove.Parameters.AddWithValue("@uid", userId);
                            cmdMove.Parameters.AddWithValue("@changed", -d.Quantity);
                            cmdMove.Parameters.AddWithValue("@after", qtyAfter);
                            cmdMove.Parameters.AddWithValue("@note",
                                "Xuat hang - Phieu #" + orderId);
                            cmdMove.ExecuteNonQuery();
                        }

                        tran.Commit();
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}