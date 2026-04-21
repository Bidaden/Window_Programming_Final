using MySellerApp.Common;
using MySellerApp.DAL.Helpers;
using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MySellerApp.DAL
{
    public class ImportOrderRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<ImportOrder> GetAll()
        {
            var list = new List<ImportOrder>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT io.Id, io.ImportDate, io.Note, u.Name AS UserName, s.Name AS Supplier " +
                    "FROM ImportOrders io JOIN Users u ON io.UserId = u.Id " +
                    "JOIN Suppliers s ON io.SupplierId = s.Id ORDER BY io.ImportDate DESC", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ImportOrder
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Supplier = reader.GetString(reader.GetOrdinal("Supplier")),
                            ImportDate = reader.GetDateTime(reader.GetOrdinal("ImportDate")),
                            Note = reader.IsDBNull(reader.GetOrdinal("Note")) ? "" : reader.GetString(reader.GetOrdinal("Note"))
                        });
                    }
                }
            }
            return list;
        }

        public void Create(int supplierId, int userId, string note, List<ImportOrderDetail> details, ProductRepository productRepo)
        {
            TransactionHelper.Execute(_conn, (con, tran) =>
            {
                var cmdOrder = new SqlCommand("INSERT INTO ImportOrders (UserId, SupplierId, Note) VALUES (@uid, @sid, @note)", con, tran);
                cmdOrder.Parameters.AddWithValue("@uid", userId);
                cmdOrder.Parameters.AddWithValue("@sid", supplierId);
                cmdOrder.Parameters.AddWithValue("@note", note ?? "");
                int orderId = TransactionHelper.GetIdentity(cmdOrder);

                foreach (var d in details)
                {
                    var cmdDetail = new SqlCommand("INSERT INTO ImportOrderDetails (ImportOrderId, ProductId, Quantity, UnitPrice) VALUES (@oid, @pid, @qty, @price)", con, tran);
                    cmdDetail.Parameters.AddWithValue("@oid", orderId);
                    cmdDetail.Parameters.AddWithValue("@pid", d.ProductId);
                    cmdDetail.Parameters.AddWithValue("@qty", d.Quantity);
                    cmdDetail.Parameters.AddWithValue("@price", d.UnitPrice);
                    cmdDetail.ExecuteNonQuery();

                    var cmdStock = new SqlCommand("UPDATE Products SET Quantity = Quantity + @qty WHERE Id = @pid", con, tran);
                    cmdStock.Parameters.AddWithValue("@qty", d.Quantity);
                    cmdStock.Parameters.AddWithValue("@pid", d.ProductId);
                    cmdStock.ExecuteNonQuery();

                    var cmdAfter = new SqlCommand("SELECT Quantity FROM Products WHERE Id = @pid", con, tran);
                    cmdAfter.Parameters.AddWithValue("@pid", d.ProductId);
                    int qtyAfter = Convert.ToInt32(cmdAfter.ExecuteScalar());

                    var cmdMove = new SqlCommand("INSERT INTO StockMovements (ProductId, UserId, Type, QuantityChanged, QuantityAfter, Note) VALUES (@pid, @uid, @type, @changed, @after, @note)", con, tran);
                    cmdMove.Parameters.AddWithValue("@pid", d.ProductId);
                    cmdMove.Parameters.AddWithValue("@uid", userId);
                    cmdMove.Parameters.AddWithValue("@type", AppConstants.STOCK_TYPE_IMPORT);
                    cmdMove.Parameters.AddWithValue("@changed", d.Quantity);
                    cmdMove.Parameters.AddWithValue("@after", qtyAfter);
                    cmdMove.Parameters.AddWithValue("@note", $"Nhập hàng - Phiếu #{orderId}");
                    cmdMove.ExecuteNonQuery();
                }
            });
        }
    }
}