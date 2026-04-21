using System;
using System.Collections.Generic;
using MySellerApp.BL.Interfaces;
using MySellerApp.Models;

namespace MySellerApp.BL
{
    public class UserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        public void Register(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Tên không được để trống!");
            if (!email.Contains("@"))
                throw new Exception("Email không hợp lệ!");
            if (password.Length < 6)
                throw new Exception("Mật khẩu phải ít nhất 6 ký tự!");
            if (_repo.EmailExists(email))
                throw new Exception("Email này đã được đăng ký!");

            _repo.Register(new User
            {
                Name = name,
                Email = email,
                Password = password,
                Role = "user" 
            });
        }

        public User Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new Exception("Vui lòng nhập đầy đủ thông tin!");

            var user = _repo.Login(email, password);
            if (user == null)
                throw new Exception("Email hoặc mật khẩu không đúng!");

            return user;
        }

        public List<User> GetAllUsers()
        {
            return _repo.GetAllUsers();
        }

        public void DeleteUser(int id, string adminRole)
        {
            if (adminRole != "super")
                throw new Exception("Bạn không có quyền xóa người dùng!");
            _repo.DeleteUser(id);
        }
    }
}