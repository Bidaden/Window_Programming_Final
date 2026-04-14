using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly string _conn = DBConnection.ConnStr;

        public bool EmailExists(string email)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Users WHERE Email = @e", con);
                cmd.Parameters.AddWithValue("@e", email);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public void Register(User user)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Users (Name, Email, Password, Role) " +
                    "VALUES (@n, @e, @p, @r)", con);
                cmd.Parameters.AddWithValue("@n", user.Name);
                cmd.Parameters.AddWithValue("@e", user.Email);
                cmd.Parameters.AddWithValue("@p", user.Password);
                cmd.Parameters.AddWithValue("@r", "user");
                cmd.ExecuteNonQuery();
            }
        }

        public User Login(string email, string password)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "SELECT * FROM Users WHERE Email = @e AND Password = @p", con);
                cmd.Parameters.AddWithValue("@e", email);
                cmd.Parameters.AddWithValue("@p", password);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Role = reader.GetString(reader.GetOrdinal("Role"))
                        };
                    }
                }
            }
            return null;
        }

        public List<User> GetAllUsers()
        {
            var list = new List<User>();
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT * FROM Users", con);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Role = reader.GetString(reader.GetOrdinal("Role"))
                        });
                    }
                }
            }
            return list;
        }

        public void DeleteUser(int id)
        {
            using (var con = new SqlConnection(_conn))
            {
                con.Open();
                var cmd = new SqlCommand(
                    "DELETE FROM Users WHERE Id = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
    }
}