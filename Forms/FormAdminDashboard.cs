using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.BL;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormAdminDashboard : Form
    {
        private readonly UserService _userService
            = new UserService(new UserRepository());
        private readonly User _admin;

        public FormAdminDashboard(User admin)
        {
            InitializeComponent();
            _admin = admin;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Admin Dashboard";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // Tiêu đề
            var lblTitle = new Label();
            lblTitle.Text = "ADMIN DASHBOARD — " + _admin.Name.ToUpper();
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(660, 36);
            lblTitle.Location = new Point(20, 15);

            // Label role
            var lblRole = new Label();
            lblRole.Text = "Cấp quyền: " + (_admin.Role == "super" ? "Super Admin" : "Normal Admin");
            lblRole.Font = new Font("Segoe UI", 9);
            lblRole.ForeColor = _admin.Role == "super" ? Color.DarkGreen : Color.DarkOrange;
            lblRole.Location = new Point(20, 55);
            lblRole.AutoSize = true;

            // DataGridView danh sách users
            var dgvUsers = new DataGridView();
            dgvUsers.Name = "dgvUsers";
            dgvUsers.Size = new Size(650, 300);
            dgvUsers.Location = new Point(20, 80);
            dgvUsers.ReadOnly = true;
            dgvUsers.AllowUserToAddRows = false;
            dgvUsers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsers.BackgroundColor = Color.White;

            // Nút Load Users
            var btnLoad = new Button();
            btnLoad.Text = "Tải danh sách User";
            btnLoad.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLoad.Size = new Size(200, 36);
            btnLoad.Location = new Point(20, 395);
            btnLoad.BackColor = Color.FromArgb(30, 80, 160);
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Cursor = Cursors.Hand;

            // Nút Xóa User
            var btnDelete = new Button();
            btnDelete.Text = "Xóa User đã chọn";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Size = new Size(200, 36);
            btnDelete.Location = new Point(235, 395);
            btnDelete.BackColor = Color.FromArgb(180, 30, 30);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;

            // Nút Đăng xuất
            var btnLogout = new Button();
            btnLogout.Text = "Đăng xuất";
            btnLogout.Font = new Font("Segoe UI", 9);
            btnLogout.Size = new Size(200, 36);
            btnLogout.Location = new Point(450, 395);
            btnLogout.BackColor = Color.FromArgb(240, 240, 240);
            btnLogout.ForeColor = Color.FromArgb(60, 60, 60);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Cursor = Cursors.Hand;

            this.Controls.AddRange(new Control[] {
                lblTitle, lblRole, dgvUsers,
                btnLoad, btnDelete, btnLogout
            });

            // ===== SỰ KIỆN =====
            btnLoad.Click += (s, e) =>
            {
                try
                {
                    dgvUsers.DataSource = _userService.GetAllUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            btnDelete.Click += (s, e) =>
            {
                if (dgvUsers.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn user cần xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int selectedId = Convert.ToInt32(dgvUsers.SelectedRows[0].Cells["Id"].Value);

                var confirm = MessageBox.Show(
                    "Bạn có chắc muốn xóa user này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        _userService.DeleteUser(selectedId, _admin.Role);
                        MessageBox.Show("Đã xóa thành công!", "OK",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgvUsers.DataSource = _userService.GetAllUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            };

            btnLogout.Click += (s, e) => this.Close();

            // Tự động load khi mở form
            dgvUsers.DataSource = _userService.GetAllUsers();
        }
    }
}