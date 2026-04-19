using System;
using System.Collections.Generic;
using System.Linq;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class SupplierRepositoryEF
    {
        public List<Supplier> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Suppliers.ToList();
            }
        }

        public int Add(Supplier s)
        {
            using (var db = new AppDbContext())
            {
                db.Suppliers.Add(s);
                db.SaveChanges();
                return s.Id;
            }
        }

        public void Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var sup = db.Suppliers.Find(id);
                if (sup != null)
                {
                    db.Suppliers.Remove(sup);
                    db.SaveChanges();
                }
            }
        }
    }
}