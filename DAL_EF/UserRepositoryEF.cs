using System;
using System.Collections.Generic;
using System.Linq;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class UserRepositoryEF : IUserRepository
    {
        public bool EmailExists(string email)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.Any(u => u.Email == email);
            }
        }

        public void Register(User user)
        {
            using (var db = new AppDbContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
        }

        public User Login(string email, string password)
        {
            using (var db = new AppDbContext())
            {
                return db.Users.FirstOrDefault(
                    u => u.Email == email && u.Password == password);
            }
        }

        public List<User> GetAllUsers()
        {
            using (var db = new AppDbContext())
            {
                return db.Users.ToList();
            }
        }

        public void DeleteUser(int id)
        {
            using (var db = new AppDbContext())
            {
                var user = db.Users.Find(id);
                if (user != null)
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                }
            }
        }
    }
}