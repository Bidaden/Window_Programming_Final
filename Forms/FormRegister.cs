using MySellerApp.BL;
using MySellerApp.DAL;
using MySellerApp.DAL_EF;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MySellerApp.Forms
{
    public partial class FormRegister : Form
    {
        private readonly UserService _userService
    = new UserService(RepositoryFactory.CreateUserRepository());

        public FormRegister()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Đăng Ký Tài Khoản";
            this.Size = new Size(420, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Logo
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
            lblTitle.Text = "ĐĂNG KÝ TÀI KHOẢN MỚI";
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

            // Label Confirm Password
            var lblConfirm = new Label();
            lblConfirm.Text = "Nhập lại Mật khẩu:";
            lblConfirm.Font = new Font("Segoe UI", 9);
            lblConfirm.Location = new Point(215, 242);
            lblConfirm.AutoSize = true;

            // TextBox Password
            var txtPassword = new TextBox();
            txtPassword.Name = "txtPassword";
            txtPassword.Font = new Font("Segoe UI", 11);
            txtPassword.Size = new Size(175, 36);
            txtPassword.Location = new Point(30, 263);
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.PasswordChar = '●';

            // TextBox Confirm Password
            var txtConfirm = new TextBox();
            txtConfirm.Name = "txtConfirm";
            txtConfirm.Font = new Font("Segoe UI", 11);
            txtConfirm.Size = new Size(175, 36);
            txtConfirm.Location = new Point(215, 263);
            txtConfirm.BorderStyle = BorderStyle.FixedSingle;
            txtConfirm.PasswordChar = '●';

            // Nút Đăng Ký
            var btnRegister = new Button();
            btnRegister.Text = "ĐĂNG KÝ";
            btnRegister.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnRegister.Size = new Size(360, 46);
            btnRegister.Location = new Point(30, 320);
            btnRegister.BackColor = Color.FromArgb(30, 80, 160);
            btnRegister.ForeColor = Color.White;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Cursor = Cursors.Hand;

            // Nút Guest
            var btnGuest = new Button();
            btnGuest.Text = "Hoặc xem với tư cách Khách (Viewer)";
            btnGuest.Font = new Font("Segoe UI", 9);
            btnGuest.Size = new Size(360, 36);
            btnGuest.Location = new Point(30, 376);
            btnGuest.BackColor = Color.FromArgb(240, 240, 240);
            btnGuest.ForeColor = Color.FromArgb(60, 60, 60);
            btnGuest.FlatStyle = FlatStyle.Flat;
            btnGuest.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnGuest.Cursor = Cursors.Hand;

            // Link Đã có tài khoản
            var lnkLogin = new LinkLabel();
            lnkLogin.Text = "Đã có tài khoản? Đăng nhập";
            lnkLogin.Font = new Font("Segoe UI", 9);
            lnkLogin.Size = new Size(360, 22);
            lnkLogin.Location = new Point(30, 460);
            lnkLogin.TextAlign = ContentAlignment.MiddleCenter;
            lnkLogin.LinkColor = Color.FromArgb(30, 80, 160);

            this.Controls.AddRange(new Control[] {
                lblLogo, lblShop, lblTitle,
                lblEmail, txtEmail,
                lblPass, lblConfirm,
                txtPassword, txtConfirm,
                btnRegister, btnGuest, lnkLogin
            });

            // ===== SỰ KIỆN =====
            btnRegister.Click += (s, e) =>
            {
                // Kiểm tra confirm password
                if (txtPassword.Text != txtConfirm.Text)
                {
                    MessageBox.Show("Mật khẩu nhập lại không khớp!", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    _userService.Register(txtEmail.Text, txtEmail.Text, txtPassword.Text);
                    MessageBox.Show("Đăng ký thành công! Vui lòng đăng nhập.", "Thành công",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi đăng ký",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnGuest.Click += (s, e) =>
            {
                this.Hide();
                new FormGuestDashboard().ShowDialog();
                this.Show();
            };

            lnkLogin.LinkClicked += (s, e) => this.Close();

            this.AcceptButton = btnRegister;
        }
    }
}