using System.Collections.Generic;
using System.Linq;
using MySellerApp.Models;

namespace MySellerApp.DAL_EF
{
    public class CategoryRepositoryEF
    {
        public List<Category> GetAll()
        {
            using (var db = new AppDbContext())
            {
                return db.Categories.ToList();
            }
        }

        public void Add(Category c)
        {
            using (var db = new AppDbContext())
            {
                db.Categories.Add(c);
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (var db = new AppDbContext())
            {
                var cat = db.Categories.Find(id);
                if (cat != null)
                {
                    db.Categories.Remove(cat);
                    db.SaveChanges();
                }
            }
        }
    }
}