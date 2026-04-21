using MySellerApp.DAL;
using MySellerApp.Forms.Helpers;
using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MySellerApp.Forms
{
    public partial class FormImportOrder : Form
    {
        private readonly ImportOrderRepository _importRepo
            = new ImportOrderRepository();
        private readonly ProductRepository _productRepo
            = new ProductRepository();
        private readonly SupplierRepository _supplierRepo
            = new SupplierRepository();

        private DataGridView _dgvOrders;

        public FormImportOrder()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Quan ly phieu nhap hang";
            this.Size = new Size(1000, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            this.Controls.Add(UIHelper.CreateHeader("QUAN LY PHIEU NHAP HANG", Color.FromArgb(30, 80, 160)));

            var pnlToolbar = new Panel { BackColor = Color.FromArgb(245, 248, 255), Dock = DockStyle.Top, Height = 48 };
            pnlToolbar.Controls.AddRange(new Control[] {
        UIHelper.CreateBtn("Tao phieu moi", Color.FromArgb(30, 160, 80), (s,e) => ShowCreateImportDialog(), 12, 7, 180),
        UIHelper.CreateBtn("Lam moi", Color.FromArgb(240, 240, 240), (s,e) => LoadData(), 202, 7, 100)
    });
            this.Controls.Add(pnlToolbar);

            _dgvOrders = UIHelper.CreateGrid();
            _dgvOrders.Dock = DockStyle.Fill;
            this.Controls.Add(_dgvOrders);
        }

        private void LoadData()
        {
            try
            {
                _dgvOrders.DataSource = _importRepo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowCreateImportDialog()
        {
            var suppliers = _supplierRepo.GetAll();
            var products = _productRepo.GetAll();

            if (suppliers.Count == 0)
            {
                MessageBox.Show("Chua co nha cung cap! Vui long them truoc.",
                    "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dlg = new Form();
            dlg.Text = "Tao phieu nhap hang moi";
            dlg.Size = new Size(700, 580);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            // Chọn nhà cung cấp
            var lblSup = new Label();
            lblSup.Text = "Nha cung cap:";
            lblSup.Font = new Font("Segoe UI", 9);
            lblSup.Location = new Point(16, 16);
            lblSup.AutoSize = true;

            var cboSupplier = new ComboBox();
            cboSupplier.Font = new Font("Segoe UI", 10);
            cboSupplier.Size = new Size(380, 30);
            cboSupplier.Location = new Point(140, 12);
            cboSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var s in suppliers)
                cboSupplier.Items.Add(s.Name);
            cboSupplier.SelectedIndex = 0;

            // Ghi chú
            var lblNote = new Label();
            lblNote.Text = "Ghi chu:";
            lblNote.Font = new Font("Segoe UI", 9);
            lblNote.Location = new Point(16, 56);
            lblNote.AutoSize = true;

            var txtNote = new TextBox();
            txtNote.Font = new Font("Segoe UI", 10);
            txtNote.Size = new Size(530, 28);
            txtNote.Location = new Point(140, 52);
            txtNote.BorderStyle = BorderStyle.FixedSingle;

            // Label chi tiết
            var lblDetail = new Label();
            lblDetail.Text = "THEM SAN PHAM VAO PHIEU NHAP:";
            lblDetail.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblDetail.ForeColor = Color.FromArgb(30, 80, 160);
            lblDetail.Location = new Point(16, 96);
            lblDetail.AutoSize = true;

            // Chọn sản phẩm
            var lblProd = new Label();
            lblProd.Text = "San pham:";
            lblProd.Font = new Font("Segoe UI", 9);
            lblProd.Location = new Point(16, 124);
            lblProd.AutoSize = true;

            var cboProd = new ComboBox();
            cboProd.Font = new Font("Segoe UI", 10);
            cboProd.Size = new Size(280, 30);
            cboProd.Location = new Point(100, 120);
            cboProd.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var p in products)
                cboProd.Items.Add(p.Name);
            if (cboProd.Items.Count > 0)
                cboProd.SelectedIndex = 0;

            var lblQty = new Label();
            lblQty.Text = "So luong:";
            lblQty.Font = new Font("Segoe UI", 9);
            lblQty.Location = new Point(394, 124);
            lblQty.AutoSize = true;

            var txtQty = new TextBox();
            txtQty.Font = new Font("Segoe UI", 10);
            txtQty.Size = new Size(80, 30);
            txtQty.Location = new Point(462, 120);
            txtQty.BorderStyle = BorderStyle.FixedSingle;
            txtQty.Text = "1";

            var lblPrice = new Label();
            lblPrice.Text = "Don gia:";
            lblPrice.Font = new Font("Segoe UI", 9);
            lblPrice.Location = new Point(16, 164);
            lblPrice.AutoSize = true;

            var txtPrice = new TextBox();
            txtPrice.Font = new Font("Segoe UI", 10);
            txtPrice.Size = new Size(200, 30);
            txtPrice.Location = new Point(100, 160);
            txtPrice.BorderStyle = BorderStyle.FixedSingle;
            txtPrice.Text = "0";

            var btnAddItem = new Button();
            btnAddItem.Text = "Them vao phieu";
            btnAddItem.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAddItem.Size = new Size(150, 30);
            btnAddItem.Location = new Point(390, 160);
            btnAddItem.BackColor = Color.FromArgb(30, 130, 200);
            btnAddItem.ForeColor = Color.White;
            btnAddItem.FlatStyle = FlatStyle.Flat;
            btnAddItem.FlatAppearance.BorderSize = 0;
            btnAddItem.Cursor = Cursors.Hand;

            // DataGridView chi tiết phiếu
            var dgvItems = new DataGridView();
            dgvItems.Size = new Size(660, 200);
            dgvItems.Location = new Point(16, 204);
            dgvItems.ReadOnly = true;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.RowTemplate.Height = 32;
            dgvItems.ColumnHeadersHeight = 34;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colProdName", HeaderText = "San pham", FillWeight = 50 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colQty", HeaderText = "So luong", FillWeight = 20 });
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            { Name = "colPrice", HeaderText = "Don gia", FillWeight = 25 });
            dgvItems.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colDel",
                HeaderText = "",
                Text = "Xoa",
                UseColumnTextForButtonValue = true,
                FillWeight = 5
            });

            var importItems = new List<ImportOrderDetail>();

            btnAddItem.Click += (s, e) =>
            {
                if (cboProd.SelectedIndex < 0)
                {
                    MessageBox.Show("Vui long chon san pham!", "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!int.TryParse(txtQty.Text, out int qty) || qty <= 0)
                {
                    MessageBox.Show("So luong khong hop le!", "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (!decimal.TryParse(txtPrice.Text, out decimal price)
                    || price < 0)
                {
                    MessageBox.Show("Don gia khong hop le!", "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var prod = products[cboProd.SelectedIndex];
                importItems.Add(new ImportOrderDetail
                {
                    ProductId = prod.Id,
                    ProductName = prod.Name,
                    Quantity = qty,
                    UnitPrice = price
                });

                dgvItems.Rows.Add(prod.Name,
                    qty, string.Format("{0:N0} d", price));
            };

            dgvItems.CellContentClick += (s, e) =>
            {
                if (e.ColumnIndex != 3 || e.RowIndex < 0) return;
                importItems.RemoveAt(e.RowIndex);
                dgvItems.Rows.RemoveAt(e.RowIndex);
            };

            // Nút lưu phiếu
            var btnSave = new Button();
            btnSave.Text = "LUU PHIEU NHAP";
            btnSave.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSave.Size = new Size(660, 44);
            btnSave.Location = new Point(16, 420);
            btnSave.BackColor = Color.FromArgb(30, 160, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, e) =>
            {
                if (importItems.Count == 0)
                {
                    MessageBox.Show("Vui long them it nhat 1 san pham!",
                        "Thong bao", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    int supplierId = suppliers[cboSupplier.SelectedIndex].Id;
                    _importRepo.Create(supplierId, 1,
                        txtNote.Text.Trim(), importItems,
                        _productRepo);

                    MessageBox.Show("Tao phieu nhap thanh cong!\n" +
                        "Ton kho da duoc cap nhat.",
                        "Thanh cong", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadData();
                    dlg.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            dlg.Controls.AddRange(new Control[] {
                lblSup, cboSupplier,
                lblNote, txtNote,
                lblDetail,
                lblProd, cboProd,
                lblQty, txtQty,
                lblPrice, txtPrice,
                btnAddItem, dgvItems, btnSave
            });
            dlg.ShowDialog();
        }
    }
}