using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormUserDashboard : Form
    {
        private readonly User _user;

        public FormUserDashboard(User user)
        {
            InitializeComponent();
            _user = user;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Tài Khoản";
            this.Size = new Size(420, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var lblTitle = new Label();
            lblTitle.Text = "THÔNG TIN TÀI KHOẢN";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(380, 36);
            lblTitle.Location = new Point(20, 20);

            var lblInfo = new Label();
            lblInfo.Text = string.Format(
                "Xin chào: {0}\nEmail: {1}\nRole: {2}",
                _user.Name, _user.Email, _user.Role);
            lblInfo.Font = new Font("Segoe UI", 10);
            lblInfo.Size = new Size(360, 80);
            lblInfo.Location = new Point(30, 70);

            var btnLogout = new Button();
            btnLogout.Text = "ĐĂNG XUẤT";
            btnLogout.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnLogout.Size = new Size(360, 42);
            btnLogout.Location = new Point(30, 180);
            btnLogout.BackColor = Color.FromArgb(30, 80, 160);
            btnLogout.ForeColor = Color.White;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Cursor = Cursors.Hand;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblInfo, btnLogout
            });

            btnLogout.Click += (s, e) => this.Close();
        }
    }
}