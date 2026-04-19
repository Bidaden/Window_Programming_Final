using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<Product> GetAll()
        {
            var list = new List<Product>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT p.Id, p.SKU, p.Name, p.Price, p.Quantity, p.MinStock, " +
                    "c.Name AS Category, s.Name AS Supplier " +
                    "FROM Products p " +
                    "JOIN Categories c ON p.CategoryId = c.Id " +
                    "JOIN Suppliers s ON p.SupplierId = s.Id", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            SKU = reader.GetString(reader.GetOrdinal("SKU")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("MinStock")),
                            Category = reader.GetString(reader.GetOrdinal("Category")),
                            Supplier = reader.GetString(reader.GetOrdinal("Supplier"))
                        });
                    }
                }
            }
            return list;
        }

        public List<Product> SearchByName(string keyword)
        {
            var list = new List<Product>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT p.Id, p.SKU, p.Name, p.Price, p.Quantity, p.MinStock, " +
                    "c.Name AS Category, s.Name AS Supplier " +
                    "FROM Products p " +
                    "JOIN Categories c ON p.CategoryId = c.Id " +
                    "JOIN Suppliers s ON p.SupplierId = s.Id " +
                    "WHERE p.Name LIKE @k", con);
                cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            SKU = reader.GetString(reader.GetOrdinal("SKU")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                            MinStock = reader.GetInt32(reader.GetOrdinal("MinStock")),
                            Category = reader.GetString(reader.GetOrdinal("Category")),
                            Supplier = reader.GetString(reader.GetOrdinal("Supplier"))
                        });
                    }
                }
            }
            return list;
        }
        public void Add(Product p)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Products " +
                    "(SKU, Name, CategoryId, SupplierId, Price, Quantity, MinStock) " +
                    "VALUES (@sku,@name,@cat,@sup,@price,@qty,@min)", con);
                cmd.Parameters.AddWithValue("@sku", p.SKU);
                cmd.Parameters.AddWithValue("@name", p.Name);
                cmd.Parameters.AddWithValue("@cat", p.CategoryId);
                cmd.Parameters.AddWithValue("@sup", p.SupplierId);
                cmd.Parameters.AddWithValue("@price", p.Price);
                cmd.Parameters.AddWithValue("@qty", p.Quantity);
                cmd.Parameters.AddWithValue("@min", p.MinStock);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateStock(int productId, int change)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "UPDATE Products SET Quantity = Quantity + @change " +
                    "WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@change", change);
                cmd.Parameters.AddWithValue("@id", productId);
                cmd.ExecuteNonQuery();
            }
        }

        public void LogStockMovement(int productId, int userId,
            string type, int changed, int after, string note)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO StockMovements " +
                    "(ProductId, UserId, Type, QuantityChanged, QuantityAfter, Note) " +
                    "VALUES (@pid,@uid,@type,@changed,@after,@note)", con);
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@changed", changed);
                cmd.Parameters.AddWithValue("@after", after);
                cmd.Parameters.AddWithValue("@note", note);
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Products WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}