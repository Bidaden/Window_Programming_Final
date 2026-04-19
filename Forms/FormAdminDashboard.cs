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
        private Panel _contentPanel;

        public FormAdminDashboard(User admin)
        {
            InitializeComponent();
            _admin = admin;
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Quản lý";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.MinimumSize = new Size(1000, 600);

            // ===== SIDEBAR =====
            var sidebar = new Panel();
            sidebar.BackColor = Color.FromArgb(25, 60, 130);
            sidebar.Dock = DockStyle.Left;
            sidebar.Width = 220;

            // Logo trên sidebar
            var pnlLogo = new Panel();
            pnlLogo.BackColor = Color.FromArgb(20, 50, 110);
            pnlLogo.Size = new Size(220, 80);
            pnlLogo.Location = new Point(0, 0);
            pnlLogo.Dock = DockStyle.Top;

            var lblLogo = new Label();
            lblLogo.Text = "ELECTRONIC";
            lblLogo.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblLogo.ForeColor = Color.White;
            lblLogo.Location = new Point(0, 18);
            lblLogo.Size = new Size(220, 26);
            lblLogo.TextAlign = ContentAlignment.MiddleCenter;

            var lblLogoSub = new Label();
            lblLogoSub.Text = "SHOP MANAGER";
            lblLogoSub.Font = new Font("Segoe UI", 8);
            lblLogoSub.ForeColor = Color.FromArgb(180, 200, 240);
            lblLogoSub.Location = new Point(0, 44);
            lblLogoSub.Size = new Size(220, 20);
            lblLogoSub.TextAlign = ContentAlignment.MiddleCenter;

            pnlLogo.Controls.AddRange(new Control[] { lblLogo, lblLogoSub });

            // Info admin
            var pnlAdmin = new Panel();
            pnlAdmin.BackColor = Color.FromArgb(30, 70, 145);
            pnlAdmin.Size = new Size(220, 60);
            pnlAdmin.Location = new Point(0, 80);

            var lblAdminName = new Label();
            lblAdminName.Text = _admin.Name;
            lblAdminName.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblAdminName.ForeColor = Color.White;
            lblAdminName.Location = new Point(12, 10);
            lblAdminName.Size = new Size(196, 20);

            var lblAdminRole = new Label();
            lblAdminRole.Text = _admin.Role == "super"
                                     ? "Super Admin" : "Normal Admin";
            lblAdminRole.Font = new Font("Segoe UI", 8);
            lblAdminRole.ForeColor = Color.FromArgb(180, 210, 255);
            lblAdminRole.Location = new Point(12, 32);
            lblAdminRole.Size = new Size(196, 18);

            pnlAdmin.Controls.AddRange(new Control[] {
                lblAdminName, lblAdminRole
            });

            // Menu buttons
            string[,] menuItems = {
                { "Tổng quan",        "dashboard"  },
                { "Sản phẩm",         "products"   },
                { "Nhập hàng",        "import"     },
                { "Xuất hàng",        "export"     },
                { "Tồn kho",          "stock"      },
                  { "Lịch sử bán hàng", "salehistory"},
                { "Danh mục",         "categories" },
                { "Nhà cung cấp",     "suppliers"  },
                { "Quản lý User",     "users"      }
            };

            int menuStartY = 155;
            for (int i = 0; i < menuItems.GetLength(0); i++)
            {
                var btn = new Button();
                btn.Text = "  " + menuItems[i, 0];
                btn.Tag = menuItems[i, 1];
                btn.Font = new Font("Segoe UI", 10);
                btn.ForeColor = Color.FromArgb(200, 220, 255);
                btn.BackColor = Color.Transparent;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor
                                               = Color.FromArgb(40, 90, 170);
                btn.Size = new Size(220, 44);
                btn.Location = new Point(0, menuStartY + i * 44);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Cursor = Cursors.Hand;

                btn.Click += MenuButton_Click;
                sidebar.Controls.Add(btn);
            }

            // Nút đăng xuất
            var btnLogout = new Button();
            btnLogout.Text = "  Đăng xuất";
            btnLogout.Font = new Font("Segoe UI", 10);
            btnLogout.ForeColor = Color.FromArgb(255, 180, 180);
            btnLogout.BackColor = Color.Transparent;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.FlatAppearance.MouseOverBackColor
                                    = Color.FromArgb(120, 40, 40);
            btnLogout.Size = new Size(220, 44);
            btnLogout.Location = new Point(0, 510);
            btnLogout.TextAlign = ContentAlignment.MiddleLeft;
            btnLogout.Cursor = Cursors.Hand;
            btnLogout.Click += (s, e) => this.Close();

            sidebar.Controls.AddRange(new Control[] {
                pnlLogo, pnlAdmin, btnLogout
            });

            // ===== CONTENT PANEL =====
            _contentPanel = new Panel();
            _contentPanel.BackColor = Color.FromArgb(245, 245, 245);
            _contentPanel.Dock = DockStyle.Fill;

            this.Controls.Add(_contentPanel);
            this.Controls.Add(sidebar);

            // Mở Dashboard mặc định
            ShowDashboard();
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            switch (btn.Tag.ToString())
            {
                case "dashboard": ShowDashboard(); break;
                case "products":
                    OpenChildForm(new FormProducts()); break;
                case "import":
                    OpenChildForm(new FormImportOrder()); break;
                case "export":
                    OpenChildForm(new FormExportOrder()); break;
                case "stock":
                    OpenChildForm(new FormStockReport()); break;
                case "categories":
                    OpenChildForm(new FormCategories()); break;
                case "suppliers":
                    OpenChildForm(new FormSuppliers()); break;
                case "users": ShowUsers(); break;
                case "salehistory":
                    OpenChildForm(new FormSaleHistory()); break;
            }
        }

        private void OpenChildForm(Form childForm)
        {
            _contentPanel.Controls.Clear();
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            _contentPanel.Controls.Add(childForm);
            childForm.Show();
        }

        private void ShowDashboard()
        {
            _contentPanel.Controls.Clear();

            var lblWelcome = new Label();
            lblWelcome.Text = "Chào mừng, " + _admin.Name + "!";
            lblWelcome.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(30, 60, 130);
            lblWelcome.Location = new Point(30, 30);
            lblWelcome.AutoSize = true;

            var lblSub = new Label();
            lblSub.Text = "Hệ thống quản lý cửa hàng điện tử";
            lblSub.Font = new Font("Segoe UI", 10);
            lblSub.ForeColor = Color.Gray;
            lblSub.Location = new Point(32, 65);
            lblSub.AutoSize = true;

            // Stat cards
            var stats = new string[,] {
                { "Sản phẩm",     GetCount("Products"),     "30, 80, 160"  },
                { "Danh mục",     GetCount("Categories"),   "40, 140, 80"  },
                { "Nhà cung cấp", GetCount("Suppliers"),    "140, 80, 160" },
                { "Người dùng",   GetCount("Users"),        "160, 100, 30" }
            };

            for (int i = 0; i < stats.GetLength(0); i++)
            {
                var rgb = stats[i, 2].Split(',');
                var color = Color.FromArgb(
                    int.Parse(rgb[0].Trim()),
                    int.Parse(rgb[1].Trim()),
                    int.Parse(rgb[2].Trim()));

                var card = new Panel();
                card.Size = new Size(180, 100);
                card.Location = new Point(30 + i * 200, 110);
                card.BackColor = color;

                var lblNum = new Label();
                lblNum.Text = stats[i, 1];
                lblNum.Font = new Font("Segoe UI", 28, FontStyle.Bold);
                lblNum.ForeColor = Color.White;
                lblNum.Location = new Point(16, 16);
                lblNum.AutoSize = true;

                var lblName = new Label();
                lblName.Text = stats[i, 0];
                lblName.Font = new Font("Segoe UI", 9);
                lblName.ForeColor = Color.FromArgb(220, 235, 255);
                lblName.Location = new Point(16, 62);
                lblName.AutoSize = true;

                card.Controls.AddRange(new Control[] { lblNum, lblName });
                _contentPanel.Controls.Add(card);
            }

            _contentPanel.Controls.AddRange(new Control[] {
                lblWelcome, lblSub
            });
        }

        private string GetCount(string tableName)
        {
            try
            {
                using (var con = new System.Data.SqlClient.SqlConnection(
                    DAL.DBConnection.ConnStr))
                {
                    con.Open();
                    var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT COUNT(*) FROM " + tableName, con);
                    return cmd.ExecuteScalar().ToString();
                }
            }
            catch { return "?"; }
        }

        private void ShowUsers()
        {
            _contentPanel.Controls.Clear();

            var lblTitle = new Label();
            lblTitle.Text = "QUẢN LÝ NGƯỜI DÙNG";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(30, 60, 130);
            lblTitle.Location = new Point(20, 20);
            lblTitle.AutoSize = true;

            var dgv = new DataGridView();
            dgv.Size = new Size(_contentPanel.Width - 40, 450);
            dgv.Location = new Point(20, 60);
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.DataSource = _userService.GetAllUsers();

            var btnDelete = new Button();
            btnDelete.Text = "Xóa User đã chọn";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Size = new Size(180, 36);
            btnDelete.Location = new Point(20, 525);
            btnDelete.BackColor = Color.FromArgb(180, 30, 30);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;

            btnDelete.Click += (s, e) =>
            {
                if (dgv.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn user cần xóa!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int id = Convert.ToInt32(dgv.SelectedRows[0].Cells["Id"].Value);
                var confirm = MessageBox.Show(
                    "Bạn có chắc muốn xóa user này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        _userService.DeleteUser(id, _admin.Role);
                        MessageBox.Show("Đã xóa!", "OK",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dgv.DataSource = _userService.GetAllUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            };

            _contentPanel.Controls.AddRange(new Control[] {
                lblTitle, dgv, btnDelete
            });
        }
    }
}