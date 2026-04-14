using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.BL;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormLogin : Form
    {
        private readonly UserService _userService
            = new UserService(new UserRepository());

        public FormLogin()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Form
            this.Text = "ElectronicShop - Đăng Nhập";
            this.Size = new Size(420, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Logo label (thay icon)
            var lblLogo = new Label();
            lblLogo.Text = "⚙";
            lblLogo.Font = new Font("Segoe UI", 36);
            lblLogo.ForeColor = Color.FromArgb(30, 80, 160);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;
            lblLogo.Size = new Size(380, 70);
            lblLogo.Location = new Point(20, 20);

            // Tên shop
            var lblShop = new Label();
            lblShop.Text = "ELECTRONIC SHOP";
            lblShop.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblShop.ForeColor = Color.FromArgb(20, 20, 20);
            lblShop.TextAlign = ContentAlignment.MiddleCenter;
            lblShop.Size = new Size(380, 30);
            lblShop.Location = new Point(20, 95);

            // Tiêu đề
            var lblTitle = new Label();
            lblTitle.Text = "ĐĂNG NHẬP";
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(20, 20, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(380, 28);
            lblTitle.Location = new Point(20, 128);

            // Label Email
            var lblEmail = new Label();
            lblEmail.Text = "Email / Tên đăng nhập:";
            lblEmail.Font = new Font("Segoe UI", 9);
            lblEmail.Location = new Point(30, 172);
            lblEmail.AutoSize = true;

            // TextBox Email
            var txtEmail = new TextBox();
            txtEmail.Name = "txtEmail";
            txtEmail.Font = new Font("Segoe UI", 11);
            txtEmail.Size = new Size(360, 36);
            txtEmail.Location = new Point(30, 193);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;

            // Label Password
            var lblPass = new Label();
            lblPass.Text = "Mật khẩu:";
            lblPass.Font = new Font("Segoe UI", 9);
            lblPass.Location = new Point(30, 242);
            lblPass.AutoSize = true;

            // TextBox Password
            var txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.Size = new Size(360, 36);
            txtPassword.Location = new Point(30, 263);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '●';

            // Nút Đăng Nhập
            var btnLogin = new Button();
            btnLogin.Text = "ĐĂNG NHẬP";
            btnLogin.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnLogin.Size = new Size(360, 46);
            btnLogin.Location = new Point(30, 318);
            btnLogin.BackColor = Color.FromArgb(30, 80, 160);
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;

            // Nút Guest
            var btnGuest = new Button();
            btnGuest.Text = "Hoặc xem với tư cách Khách (Viewer)";
            btnGuest.Font = new Font("Segoe UI", 9);
            btnGuest.Size = new Size(360, 36);
            btnGuest.Location = new Point(30, 374);
            btnGuest.BackColor = Color.FromArgb(240, 240, 240);
            btnGuest.ForeColor = Color.FromArgb(60, 60, 60);
            btnGuest.FlatStyle = FlatStyle.Flat;
            btnGuest.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnGuest.Cursor = Cursors.Hand;

            // Link Đăng ký
            var lnkRegister = new LinkLabel();
            lnkRegister.Text = "Chưa có tài khoản? Đăng ký ngay";
            lnkRegister.Font = new Font("Segoe UI", 9);
            lnkRegister.Size = new Size(360, 22);
            lnkRegister.Location = new Point(30, 455);
            lnkRegister.TextAlign = ContentAlignment.MiddleCenter;
            lnkRegister.LinkColor = Color.FromArgb(30, 80, 160);

            // Thêm controls vào Form
            this.Controls.AddRange(new Control[] {
                lblLogo, lblShop, lblTitle,
                lblEmail, txtEmail,
                lblPass, txtPassword,
                btnLogin, btnGuest, lnkRegister
            });

            // ===== SỰ KIỆN =====
            btnLogin.Click += (s, e) =>
            {
                try
                {
                    User user = _userService.Login(txtEmail.Text, txtPassword.Text);

                    this.Hide();

                    if (user.Role == "super" || user.Role == "normal")
                        new FormAdminDashboard(user).ShowDialog();
                    else
                        new FormUserDashboard(user).ShowDialog();

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi đăng nhập",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnGuest.Click += (s, e) =>
            {
                this.Hide();
                new FormGuestDashboard().ShowDialog();
                this.Show();
            };

            lnkRegister.LinkClicked += (s, e) =>
            {
                this.Hide();
                new FormRegister().ShowDialog();
                this.Show();
            };

            // Enter để đăng nhập
            this.AcceptButton = btnLogin;
        }


    }
}