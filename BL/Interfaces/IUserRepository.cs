using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySellerApp.Models;

namespace MySellerApp.BL.Interfaces
{
    public interface IUserRepository
    {
        bool EmailExists(string email);
        void Register(User user);
        User Login(string email, string password);
        List<User> GetAllUsers();
        void DeleteUser(int id);
    }
}