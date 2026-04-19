using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MySellerApp.DAL
{
    public class SupplierRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public List<Supplier> GetAll()
        {
            var list = new List<Supplier>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM Suppliers", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Supplier
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Phone = reader.IsDBNull(reader.GetOrdinal("Phone"))
                                      ? "" : reader.GetString(reader.GetOrdinal("Phone")),
                            Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                      ? "" : reader.GetString(reader.GetOrdinal("Email")),
                            Address = reader.IsDBNull(reader.GetOrdinal("Address"))
                                      ? "" : reader.GetString(reader.GetOrdinal("Address"))
                        });
                    }
                }
            }
            return list;
        }
        public int Add(Supplier s)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Suppliers (Name, Phone, Email, Address) " +
                    "VALUES (@name,@phone,@email,@addr); " +
                    "SELECT SCOPE_IDENTITY();", con);
                cmd.Parameters.AddWithValue("@name", s.Name);
                cmd.Parameters.AddWithValue("@phone", s.Phone ?? "");
                cmd.Parameters.AddWithValue("@email", s.Email ?? "");
                cmd.Parameters.AddWithValue("@addr", s.Address ?? "");
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
        public void Delete(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Suppliers WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}