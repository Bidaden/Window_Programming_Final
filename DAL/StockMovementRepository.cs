using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class StockMovementRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<StockMovement> GetAll()
        {
            var list = new List<StockMovement>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT sm.Id, sm.Type, sm.QuantityChanged, sm.QuantityAfter, " +
                    "sm.MovedAt, sm.Note, " +
                    "p.Name AS ProductName, u.Name AS UserName " +
                    "FROM StockMovements sm " +
                    "JOIN Products p ON sm.ProductId = p.Id " +
                    "JOIN Users u ON sm.UserId = u.Id " +
                    "ORDER BY sm.MovedAt DESC", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StockMovement
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Type = reader.GetString(reader.GetOrdinal("Type")),
                            QuantityChanged = reader.GetInt32(reader.GetOrdinal("QuantityChanged")),
                            QuantityAfter = reader.GetInt32(reader.GetOrdinal("QuantityAfter")),
                            MovedAt = reader.GetDateTime(reader.GetOrdinal("MovedAt")),
                            Note = reader.IsDBNull(reader.GetOrdinal("Note"))
                                              ? "" : reader.GetString(reader.GetOrdinal("Note"))
                        });
                    }
                }
            }
            return list;
        }
    }
}