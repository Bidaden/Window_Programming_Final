using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class CategoryRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<Category> GetAll()
        {
            var list = new List<Category>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM Categories", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Category
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                                          ? "" : reader.GetString(reader.GetOrdinal("Description"))
                        });
                    }
                }
            }
            return list;
        }
        public void Add(Category c)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Categories (Name, Description) " +
                    "VALUES (@name, @desc)", con);
                cmd.Parameters.AddWithValue("@name", c.Name);
                cmd.Parameters.AddWithValue("@desc", c.Description ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Categories WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}