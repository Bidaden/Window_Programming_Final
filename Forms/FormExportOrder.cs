using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.DAL;

namespace MySellerApp.Forms
{
    public partial class FormExportOrder : Form
    {
        private readonly ExportOrderRepository _repo = new ExportOrderRepository();
        private DataGridView dgvOrders;

        public FormExportOrder()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Phiếu xuất hàng";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var lblTitle = new Label();
            lblTitle.Text = "QUẢN LÝ PHIẾU XUẤT HÀNG";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(860, 36);
            lblTitle.Location = new Point(20, 15);

            dgvOrders = new DataGridView();
            dgvOrders.Size = new Size(860, 450);
            dgvOrders.Location = new Point(20, 60);
            dgvOrders.ReadOnly = true;
            dgvOrders.AllowUserToAddRows = false;
            dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOrders.BackgroundColor = Color.White;

            var btnLoad = new Button();
            btnLoad.Text = "Tải phiếu xuất";
            btnLoad.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLoad.Size = new Size(150, 36);
            btnLoad.Location = new Point(20, 525);
            btnLoad.BackColor = Color.FromArgb(30, 80, 160);
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Cursor = Cursors.Hand;

            this.Controls.AddRange(new Control[] {
                lblTitle, dgvOrders, btnLoad
            });

            btnLoad.Click += (s, e) => LoadData();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                dgvOrders.DataSource = _repo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}