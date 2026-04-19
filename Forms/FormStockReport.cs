using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormStockReport : Form
    {
        private readonly StockMovementRepository _movRepo
            = new StockMovementRepository();
        private readonly ProductRepository _productRepo
            = new ProductRepository();

        private DataGridView _dgvStock;
        private DataGridView _dgvMovements;
        private Label _lblLowStock;

        public FormStockReport()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Bao cao ton kho";
            this.Size = new Size(1100, 680);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // ===== HEADER =====
            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "BAO CAO TON KHO";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            // ===== CẢNH BÁO HÀNG THẤP =====
            _lblLowStock = new Label();
            _lblLowStock.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _lblLowStock.ForeColor = Color.White;
            _lblLowStock.BackColor = Color.OrangeRed;
            _lblLowStock.Dock = DockStyle.Top;
            _lblLowStock.Height = 30;
            _lblLowStock.TextAlign = ContentAlignment.MiddleCenter;
            _lblLowStock.Visible = false;

            // ===== TOOLBAR =====
            var pnlToolbar = new Panel();
            pnlToolbar.BackColor = Color.FromArgb(245, 248, 255);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 44;

            var lblFilter = new Label();
            lblFilter.Text = "Loc theo loai:";
            lblFilter.Font = new Font("Segoe UI", 9);
            lblFilter.Location = new Point(12, 12);
            lblFilter.AutoSize = true;

            var cboFilter = new ComboBox();
            cboFilter.Font = new Font("Segoe UI", 9);
            cboFilter.Size = new Size(160, 28);
            cboFilter.Location = new Point(105, 8);
            cboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cboFilter.Items.AddRange(new object[] {
                "Tat ca", "IMPORT", "EXPORT", "ADJUST"
            });
            cboFilter.SelectedIndex = 0;

            var btnRefresh = new Button();
            btnRefresh.Text = "Lam moi";
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Location = new Point(280, 7);
            btnRefresh.BackColor = Color.FromArgb(240, 240, 240);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadData();

            pnlToolbar.Controls.AddRange(new Control[] {
                lblFilter, cboFilter, btnRefresh
            });

            // ===== SPLIT: trên = tồn kho, dưới = lịch sử =====
            var splitContainer = new SplitContainer();
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            splitContainer.SplitterDistance = 280;
            splitContainer.BackColor = Color.White;

            // Panel trên — tồn kho hiện tại
            var lblStock = new Label();
            lblStock.Text = "TON KHO HIEN TAI";
            lblStock.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblStock.ForeColor = Color.FromArgb(30, 60, 130);
            lblStock.Dock = DockStyle.Top;
            lblStock.Height = 28;
            lblStock.TextAlign = ContentAlignment.MiddleLeft;
            lblStock.Padding = new Padding(8, 0, 0, 0);

            _dgvStock = new DataGridView();
            _dgvStock.Dock = DockStyle.Fill;
            _dgvStock.ReadOnly = true;
            _dgvStock.AllowUserToAddRows = false;
            _dgvStock.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvStock.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvStock.BackgroundColor = Color.White;
            _dgvStock.RowTemplate.Height = 34;
            _dgvStock.ColumnHeadersHeight = 36;
            _dgvStock.MultiSelect = false;

            _dgvStock.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _dgvStock.Rows.Count) return;
                var row = _dgvStock.Rows[e.RowIndex];
                var qtyCell = row.Cells["Quantity"];
                var minCell = row.Cells["MinStock"];
                if (qtyCell?.Value == null || minCell?.Value == null) return;
                if (Convert.ToInt32(qtyCell.Value) <=
                    Convert.ToInt32(minCell.Value))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 235);
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            };

            splitContainer.Panel1.Controls.Add(_dgvStock);
            splitContainer.Panel1.Controls.Add(lblStock);

            // Panel dưới — lịch sử thay đổi
            var lblHist = new Label();
            lblHist.Text = "LICH SU THAY DOI TON KHO";
            lblHist.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblHist.ForeColor = Color.FromArgb(30, 60, 130);
            lblHist.Dock = DockStyle.Top;
            lblHist.Height = 28;
            lblHist.TextAlign = ContentAlignment.MiddleLeft;
            lblHist.Padding = new Padding(8, 0, 0, 0);

            _dgvMovements = new DataGridView();
            _dgvMovements.Dock = DockStyle.Fill;
            _dgvMovements.ReadOnly = true;
            _dgvMovements.AllowUserToAddRows = false;
            _dgvMovements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvMovements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgvMovements.BackgroundColor = Color.White;
            _dgvMovements.RowTemplate.Height = 32;
            _dgvMovements.ColumnHeadersHeight = 36;

            _dgvMovements.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0 ||
                    e.RowIndex >= _dgvMovements.Rows.Count) return;
                var row = _dgvMovements.Rows[e.RowIndex];
                var typeCell = row.Cells["Type"];
                if (typeCell?.Value == null) return;
                switch (typeCell.Value.ToString())
                {
                    case "IMPORT":
                        row.DefaultCellStyle.BackColor =
                            Color.FromArgb(235, 255, 240);
                        row.DefaultCellStyle.ForeColor = Color.DarkGreen;
                        break;
                    case "EXPORT":
                        row.DefaultCellStyle.BackColor =
                            Color.FromArgb(255, 240, 240);
                        row.DefaultCellStyle.ForeColor = Color.DarkRed;
                        break;
                    default:
                        row.DefaultCellStyle.BackColor = Color.White;
                        row.DefaultCellStyle.ForeColor = Color.Black;
                        break;
                }
            };

            splitContainer.Panel2.Controls.Add(_dgvMovements);
            splitContainer.Panel2.Controls.Add(lblHist);

            // Filter sự kiện
            cboFilter.SelectedIndexChanged += (s, e) =>
            {
                string selected = cboFilter.SelectedItem.ToString();
                LoadMovements(selected == "Tat ca" ? "" : selected);
            };

            this.Controls.Add(splitContainer);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(_lblLowStock);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            LoadStock();
            LoadMovements("");
        }

        private void LoadStock()
        {
            try
            {
                var products = _productRepo.GetAll();
                _dgvStock.DataSource = products;

                // Đếm hàng thấp
                int lowCount = 0;
                foreach (var p in products)
                    if (p.Quantity <= p.MinStock) lowCount++;

                if (lowCount > 0)
                {
                    _lblLowStock.Text = string.Format(
                        "CANH BAO: Co {0} san pham sap het hang (ton kho <= min stock)!",
                        lowCount);
                    _lblLowStock.Visible = true;
                }
                else
                {
                    _lblLowStock.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMovements(string typeFilter)
        {
            try
            {
                var movements = _movRepo.GetAll();
                if (!string.IsNullOrEmpty(typeFilter))
                    movements = movements.FindAll(
                        m => m.Type == typeFilter);
                _dgvMovements.DataSource = movements;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}