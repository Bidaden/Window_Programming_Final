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
                var cmd = new SqlCommand("SELECT * FROM Products", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
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
                    "SELECT * FROM Products WHERE Name LIKE @k", con);
                cmd.Parameters.AddWithValue("@k", "%" + keyword + "%");
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Product
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                            Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        });
                    }
                }
            }
            return list;
        }
    }
}