using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.BL;

namespace MySellerApp.Forms
{
    public partial class FormOrderHistory : Form
    {
        private readonly SaleOrderService _saleService
            = new SaleOrderService();
        private DataGridView _dgvOrders;
        private DataGridView _dgvDetails;

        public FormOrderHistory()
        {
            InitializeComponent();
            SetupUI();
            LoadOrders();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Lich su don hang";
            this.Size = new Size(850, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "LICH SU DON HANG";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            var lblOrders = new Label();
            lblOrders.Text = "Danh sach don hang:";
            lblOrders.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblOrders.ForeColor = Color.FromArgb(30, 60, 130);
            lblOrders.Location = new Point(20, 62);
            lblOrders.AutoSize = true;

            _dgvOrders = new DataGridView();
            _dgvOrders.Size = new Size(800, 200);
            _dgvOrders.Location = new Point(20, 82);
            _dgvOrders.ReadOnly = true;
            _dgvOrders.AllowUserToAddRows = false;
            _dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvOrders.BackgroundColor = Color.White;
            _dgvOrders.MultiSelect = false;
            _dgvOrders.RowTemplate.Height = 32;

            var lblDetails = new Label();
            lblDetails.Text = "Chi tiet don hang (click vao don de xem):";
            lblDetails.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblDetails.ForeColor = Color.FromArgb(30, 60, 130);
            lblDetails.Location = new Point(20, 295);
            lblDetails.AutoSize = true;

            _dgvDetails = new DataGridView();
            _dgvDetails.Size = new Size(800, 220);
            _dgvDetails.Location = new Point(20, 315);
            _dgvDetails.ReadOnly = true;
            _dgvDetails.AllowUserToAddRows = false;
            _dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvDetails.BackgroundColor = Color.White;
            _dgvDetails.RowTemplate.Height = 32;

            var btnClose = new Button();
            btnClose.Text = "Dong";
            btnClose.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnClose.Size = new Size(120, 36);
            btnClose.Location = new Point(700, 550);
            btnClose.BackColor = Color.FromArgb(30, 80, 160);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Cursor = Cursors.Hand;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                pnlHeader, lblOrders, _dgvOrders,
                lblDetails, _dgvDetails, btnClose
            });

            _dgvOrders.SelectionChanged += (s, e) =>
            {
                if (_dgvOrders.SelectedRows.Count == 0) return;
                var idCell = _dgvOrders.SelectedRows[0].Cells["Id"];
                if (idCell.Value == null) return;
                int orderId = Convert.ToInt32(idCell.Value);
                try
                {
                    _dgvDetails.DataSource =
                        _saleService.GetOrderDetails(orderId);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        private void LoadOrders()
        {
            try
            {
                _dgvOrders.DataSource = _saleService.GetAllOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}