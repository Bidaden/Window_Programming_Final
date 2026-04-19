using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.BL;

namespace MySellerApp.Forms
{
    public partial class FormSaleHistory : Form
    {
        private readonly SaleOrderService _saleService
            = new SaleOrderService();

        private DataGridView _dgvOrders;
        private DataGridView _dgvDetails;
        private Label _lblRevenue;

        public FormSaleHistory()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Lich su ban hang";
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // ===== HEADER =====
            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "LICH SU BAN HANG";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            // ===== TỔNG DOANH THU =====
            var pnlRevenue = new Panel();
            pnlRevenue.BackColor = Color.FromArgb(245, 248, 255);
            pnlRevenue.Dock = DockStyle.Top;
            pnlRevenue.Height = 44;

            var lblRevenueLabel = new Label();
            lblRevenueLabel.Text = "TONG DOANH THU:";
            lblRevenueLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblRevenueLabel.ForeColor = Color.FromArgb(30, 60, 130);
            lblRevenueLabel.Location = new Point(16, 12);
            lblRevenueLabel.AutoSize = true;

            _lblRevenue = new Label();
            _lblRevenue.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            _lblRevenue.ForeColor = Color.FromArgb(200, 50, 50);
            _lblRevenue.Location = new Point(200, 8);
            _lblRevenue.Size = new Size(400, 30);

            var btnRefresh = new Button();
            btnRefresh.Text = "Lam moi";
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Location = new Point(870, 7);
            btnRefresh.BackColor = Color.FromArgb(240, 240, 240);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadData();

            pnlRevenue.Controls.AddRange(new Control[] {
                lblRevenueLabel, _lblRevenue, btnRefresh
            });

            // ===== SPLIT =====
            var split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.SplitterDistance = 280;
            split.BackColor = Color.White;

            // Panel trên — danh sách đơn hàng
            var lblOrders = new Label();
            lblOrders.Text = "DANH SACH DON HANG DA THANH TOAN";
            lblOrders.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblOrders.ForeColor = Color.FromArgb(30, 60, 130);
            lblOrders.Dock = DockStyle.Top;
            lblOrders.Height = 28;
            lblOrders.TextAlign = ContentAlignment.MiddleLeft;
            lblOrders.Padding = new Padding(8, 0, 0, 0);

            _dgvOrders = new DataGridView();
            _dgvOrders.Dock = DockStyle.Fill;
            _dgvOrders.ReadOnly = true;
            _dgvOrders.AllowUserToAddRows = false;
            _dgvOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvOrders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvOrders.BackgroundColor = Color.White;
            _dgvOrders.RowTemplate.Height = 34;
            _dgvOrders.ColumnHeadersHeight = 36;
            _dgvOrders.MultiSelect = false;

            split.Panel1.Controls.Add(_dgvOrders);
            split.Panel1.Controls.Add(lblOrders);

            // Panel dưới — chi tiết đơn hàng
            var lblDetails = new Label();
            lblDetails.Text = "CHI TIET DON HANG (click vao don de xem)";
            lblDetails.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblDetails.ForeColor = Color.FromArgb(30, 60, 130);
            lblDetails.Dock = DockStyle.Top;
            lblDetails.Height = 28;
            lblDetails.TextAlign = ContentAlignment.MiddleLeft;
            lblDetails.Padding = new Padding(8, 0, 0, 0);

            _dgvDetails = new DataGridView();
            _dgvDetails.Dock = DockStyle.Fill;
            _dgvDetails.ReadOnly = true;
            _dgvDetails.AllowUserToAddRows = false;
            _dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvDetails.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvDetails.BackgroundColor = Color.White;
            _dgvDetails.RowTemplate.Height = 32;
            _dgvDetails.ColumnHeadersHeight = 36;

            split.Panel2.Controls.Add(_dgvDetails);
            split.Panel2.Controls.Add(lblDetails);

            // Sự kiện click đơn hàng
            _dgvOrders.SelectionChanged += (s, e) =>
            {
                if (_dgvOrders.SelectedRows.Count == 0) return;
                var idCell = _dgvOrders.SelectedRows[0].Cells["Id"];
                if (idCell?.Value == null) return;
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

            this.Controls.Add(split);
            this.Controls.Add(pnlRevenue);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try
            {
                var orders = _saleService.GetAllOrders();
                _dgvOrders.DataSource = orders;

                // Tính tổng doanh thu
                decimal total = 0;
                foreach (var o in orders) total += o.TotalAmount;
                _lblRevenue.Text = string.Format("{0:N0} d", total);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}