using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormProducts : Form
    {
        private readonly ProductRepository _productRepo
            = new ProductRepository();
        private readonly CategoryRepository _categoryRepo
            = new CategoryRepository();
        private readonly SupplierRepository _supplierRepo
            = new SupplierRepository();

        private DataGridView _dgv;
        private List<Supplier> _suppliers = new List<Supplier>();
        private List<Category> _categories = new List<Category>();

        public FormProducts()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Quan ly san pham";
            this.Size = new Size(1100, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // ===== HEADER =====
            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "QUAN LY SAN PHAM";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            // ===== TOOLBAR =====
            var pnlToolbar = new Panel();
            pnlToolbar.BackColor = Color.FromArgb(245, 248, 255);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 48;

            var btnAdd = new Button();
            btnAdd.Text = "Them san pham";
            btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAdd.Size = new Size(150, 34);
            btnAdd.Location = new Point(12, 7);
            btnAdd.BackColor = Color.FromArgb(30, 160, 80);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => ShowAddProductDialog();

            var btnIncrease = new Button();
            btnIncrease.Text = "+ Tang stock";
            btnIncrease.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnIncrease.Size = new Size(130, 34);
            btnIncrease.Location = new Point(172, 7);
            btnIncrease.BackColor = Color.FromArgb(30, 130, 200);
            btnIncrease.ForeColor = Color.White;
            btnIncrease.FlatStyle = FlatStyle.Flat;
            btnIncrease.FlatAppearance.BorderSize = 0;
            btnIncrease.Cursor = Cursors.Hand;
            btnIncrease.Click += (s, e) => AdjustStock(true);

            var btnDecrease = new Button();
            btnDecrease.Text = "- Giam stock";
            btnDecrease.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDecrease.Size = new Size(130, 34);
            btnDecrease.Location = new Point(312, 7);
            btnDecrease.BackColor = Color.FromArgb(200, 100, 30);
            btnDecrease.ForeColor = Color.White;
            btnDecrease.FlatStyle = FlatStyle.Flat;
            btnDecrease.FlatAppearance.BorderSize = 0;
            btnDecrease.Cursor = Cursors.Hand;
            btnDecrease.Click += (s, e) => AdjustStock(false);

            var btnDelete = new Button();
            btnDelete.Text = "Xoa san pham";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Size = new Size(130, 34);
            btnDelete.Location = new Point(452, 7);
            btnDelete.BackColor = Color.FromArgb(180, 30, 30);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += (s, e) => DeleteProduct();

            var btnRefresh = new Button();
            btnRefresh.Text = "Lam moi";
            btnRefresh.Font = new Font("Segoe UI", 9);
            btnRefresh.Size = new Size(100, 34);
            btnRefresh.Location = new Point(592, 7);
            btnRefresh.BackColor = Color.FromArgb(240, 240, 240);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.Click += (s, e) => LoadData();

            pnlToolbar.Controls.AddRange(new Control[] {
                btnAdd, btnIncrease, btnDecrease, btnDelete, btnRefresh
            });

            // ===== DATAGRIDVIEW =====
            _dgv = new DataGridView();
            _dgv.Dock = DockStyle.Fill;
            _dgv.ReadOnly = true;
            _dgv.AllowUserToAddRows = false;
            _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgv.BackgroundColor = Color.White;
            _dgv.RowTemplate.Height = 36;
            _dgv.ColumnHeadersHeight = 40;
            _dgv.MultiSelect = false;

            // Tô màu dòng hàng thấp
            _dgv.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _dgv.Rows.Count) return;
                var row = _dgv.Rows[e.RowIndex];
                var qtyCell = row.Cells["Quantity"];
                var minCell = row.Cells["MinStock"];
                if (qtyCell?.Value == null || minCell?.Value == null) return;
                if (Convert.ToInt32(qtyCell.Value) <=
                    Convert.ToInt32(minCell.Value))
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 240);
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            };

            this.Controls.Add(_dgv);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try
            {
                _suppliers = _supplierRepo.GetAll();
                _categories = _categoryRepo.GetAll();
                _dgv.DataSource = _productRepo.GetAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== THÊM SẢN PHẨM =====
        private void ShowAddProductDialog()
        {
            var dlg = new Form();
            dlg.Text = "Them san pham moi";
            dlg.Size = new Size(520, 620);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            int labelX = 20;
            int inputX = 160;
            int inputW = 320;
            int startY = 20;
            int gapY = 48;

            // Helper tạo label
            Action<string, int> addLabel = (text, y) =>
            {
                var lbl = new Label();
                lbl.Text = text;
                lbl.Font = new Font("Segoe UI", 9);
                lbl.Location = new Point(labelX, y + 4);
                lbl.AutoSize = true;
                dlg.Controls.Add(lbl);
            };

            // SKU
            addLabel("SKU:", startY);
            var txtSKU = new TextBox();
            txtSKU.Font = new Font("Segoe UI", 10);
            txtSKU.Size = new Size(inputW, 30);
            txtSKU.Location = new Point(inputX, startY);
            txtSKU.BorderStyle = BorderStyle.FixedSingle;

            // Tên sản phẩm
            addLabel("Ten san pham:", startY + gapY);
            var txtName = new TextBox();
            txtName.Font = new Font("Segoe UI", 10);
            txtName.Size = new Size(inputW, 30);
            txtName.Location = new Point(inputX, startY + gapY);
            txtName.BorderStyle = BorderStyle.FixedSingle;

            // Giá
            addLabel("Gia (VND):", startY + gapY * 2);
            var txtPrice = new TextBox();
            txtPrice.Font = new Font("Segoe UI", 10);
            txtPrice.Size = new Size(inputW, 30);
            txtPrice.Location = new Point(inputX, startY + gapY * 2);
            txtPrice.BorderStyle = BorderStyle.FixedSingle;
            txtPrice.Text = "0";

            // Số lượng
            addLabel("So luong:", startY + gapY * 3);
            var txtQty = new TextBox();
            txtQty.Font = new Font("Segoe UI", 10);
            txtQty.Size = new Size(inputW, 30);
            txtQty.Location = new Point(inputX, startY + gapY * 3);
            txtQty.BorderStyle = BorderStyle.FixedSingle;
            txtQty.Text = "0";

            // MinStock
            addLabel("Min Stock:", startY + gapY * 4);
            var txtMin = new TextBox();
            txtMin.Font = new Font("Segoe UI", 10);
            txtMin.Size = new Size(inputW, 30);
            txtMin.Location = new Point(inputX, startY + gapY * 4);
            txtMin.BorderStyle = BorderStyle.FixedSingle;
            txtMin.Text = "5";

            // Danh mục
            addLabel("Danh muc:", startY + gapY * 5);
            var cboCategory = new ComboBox();
            cboCategory.Font = new Font("Segoe UI", 10);
            cboCategory.Size = new Size(inputW, 30);
            cboCategory.Location = new Point(inputX, startY + gapY * 5);
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var c in _categories)
                cboCategory.Items.Add(c.Name);
            if (cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;

            // ===== NHÀ CUNG CẤP =====
            var lblSupplierHeader = new Label();
            lblSupplierHeader.Text = "NHA CUNG CAP";
            lblSupplierHeader.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblSupplierHeader.ForeColor = Color.FromArgb(30, 80, 160);
            lblSupplierHeader.Location = new Point(labelX, startY + gapY * 6 + 8);
            lblSupplierHeader.AutoSize = true;

            // ComboBox chọn nhà cung cấp cũ
            var lblOldSupplier = new Label();
            lblOldSupplier.Text = "Chon NCC cu:";
            lblOldSupplier.Font = new Font("Segoe UI", 9);
            lblOldSupplier.Location = new Point(labelX, startY + gapY * 7 + 4);
            lblOldSupplier.AutoSize = true;

            var cboSupplier = new ComboBox();
            cboSupplier.Font = new Font("Segoe UI", 10);
            cboSupplier.Size = new Size(inputW, 30);
            cboSupplier.Location = new Point(inputX, startY + gapY * 7);
            cboSupplier.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSupplier.Items.Add("-- Nhap nha cung cap moi --");
            foreach (var sup in _suppliers)
                cboSupplier.Items.Add(sup.Name);
            cboSupplier.SelectedIndex = 0;

            // Panel nhập NCC mới
            var pnlNewSupplier = new Panel();
            pnlNewSupplier.Size = new Size(480, 90);
            pnlNewSupplier.Location = new Point(labelX, startY + gapY * 8);
            pnlNewSupplier.BackColor = Color.FromArgb(245, 248, 255);
            pnlNewSupplier.Visible = true;

            var lblNewName = new Label();
            lblNewName.Text = "Ten NCC:";
            lblNewName.Font = new Font("Segoe UI", 9);
            lblNewName.Location = new Point(4, 8);
            lblNewName.AutoSize = true;

            var txtNewSupplierName = new TextBox();
            txtNewSupplierName.Font = new Font("Segoe UI", 9);
            txtNewSupplierName.Size = new Size(200, 26);
            txtNewSupplierName.Location = new Point(80, 6);
            txtNewSupplierName.BorderStyle = BorderStyle.FixedSingle;

            var lblNewPhone = new Label();
            lblNewPhone.Text = "So DT:";
            lblNewPhone.Font = new Font("Segoe UI", 9);
            lblNewPhone.Location = new Point(4, 40);
            lblNewPhone.AutoSize = true;

            var txtNewPhone = new TextBox();
            txtNewPhone.Font = new Font("Segoe UI", 9);
            txtNewPhone.Size = new Size(140, 26);
            txtNewPhone.Location = new Point(80, 38);
            txtNewPhone.BorderStyle = BorderStyle.FixedSingle;

            var lblNewEmail = new Label();
            lblNewEmail.Text = "Email:";
            lblNewEmail.Font = new Font("Segoe UI", 9);
            lblNewEmail.Location = new Point(240, 8);
            lblNewEmail.AutoSize = true;

            var txtNewEmail = new TextBox();
            txtNewEmail.Font = new Font("Segoe UI", 9);
            txtNewEmail.Size = new Size(180, 26);
            txtNewEmail.Location = new Point(285, 6);
            txtNewEmail.BorderStyle = BorderStyle.FixedSingle;

            var lblNewAddress = new Label();
            lblNewAddress.Text = "Dia chi:";
            lblNewAddress.Font = new Font("Segoe UI", 9);
            lblNewAddress.Location = new Point(240, 40);
            lblNewAddress.AutoSize = true;

            var txtNewAddress = new TextBox();
            txtNewAddress.Font = new Font("Segoe UI", 9);
            txtNewAddress.Size = new Size(180, 26);
            txtNewAddress.Location = new Point(285, 38);
            txtNewAddress.BorderStyle = BorderStyle.FixedSingle;

            pnlNewSupplier.Controls.AddRange(new Control[] {
                lblNewName, txtNewSupplierName,
                lblNewPhone, txtNewPhone,
                lblNewEmail, txtNewEmail,
                lblNewAddress, txtNewAddress
            });

            // Ẩn/hiện panel nhập NCC mới khi chọn ComboBox
            cboSupplier.SelectedIndexChanged += (s, e) =>
            {
                pnlNewSupplier.Visible = cboSupplier.SelectedIndex == 0;
            };

            // Nút Lưu
            var btnSave = new Button();
            btnSave.Text = "LUU SAN PHAM";
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Size = new Size(460, 42);
            btnSave.Location = new Point(labelX, startY + gapY * 9 + 20);
            btnSave.BackColor = Color.FromArgb(30, 160, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, e) =>
            {
                try
                {
                    // Validate
                    if (string.IsNullOrWhiteSpace(txtSKU.Text))
                        throw new Exception("Vui long nhap SKU!");
                    if (string.IsNullOrWhiteSpace(txtName.Text))
                        throw new Exception("Vui long nhap ten san pham!");
                    if (!decimal.TryParse(txtPrice.Text, out decimal price)
                        || price < 0)
                        throw new Exception("Gia khong hop le!");
                    if (!int.TryParse(txtQty.Text, out int qty) || qty < 0)
                        throw new Exception("So luong khong hop le!");
                    if (!int.TryParse(txtMin.Text, out int minStock)
                        || minStock < 0)
                        throw new Exception("Min stock khong hop le!");
                    if (cboCategory.SelectedIndex < 0)
                        throw new Exception("Vui long chon danh muc!");

                    // Xác định SupplierId
                    int supplierId;
                    if (cboSupplier.SelectedIndex == 0)
                    {
                        // Tạo nhà cung cấp mới
                        if (string.IsNullOrWhiteSpace(txtNewSupplierName.Text))
                            throw new Exception("Vui long nhap ten nha cung cap!");

                        supplierId = _supplierRepo.Add(new Supplier
                        {
                            Name = txtNewSupplierName.Text.Trim(),
                            Phone = txtNewPhone.Text.Trim(),
                            Email = txtNewEmail.Text.Trim(),
                            Address = txtNewAddress.Text.Trim()
                        });
                    }
                    else
                    {
                        supplierId = _suppliers[cboSupplier.SelectedIndex - 1].Id;
                    }

                    // Xác định CategoryId
                    int categoryId = _categories[cboCategory.SelectedIndex].Id;

                    // Thêm sản phẩm
                    _productRepo.Add(new Product
                    {
                        SKU = txtSKU.Text.Trim(),
                        Name = txtName.Text.Trim(),
                        Price = price,
                        Quantity = qty,
                        MinStock = minStock,
                        CategoryId = categoryId,
                        SupplierId = supplierId
                    });

                    MessageBox.Show("Them san pham thanh cong!", "OK",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    dlg.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            dlg.Controls.AddRange(new Control[] {
                lblSupplierHeader,
                txtSKU, txtName, txtPrice, txtQty, txtMin,
                cboCategory, lblOldSupplier, cboSupplier,
                pnlNewSupplier, btnSave
            });

            dlg.ShowDialog();
        }

        // ===== TĂNG / GIẢM STOCK =====
        private void AdjustStock(bool isIncrease)
        {
            if (_dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui long chon san pham!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = Convert.ToInt32(
                _dgv.SelectedRows[0].Cells["Id"].Value);
            string productName = _dgv.SelectedRows[0]
                .Cells["Name"].Value.ToString();
            int currentQty = Convert.ToInt32(
                _dgv.SelectedRows[0].Cells["Quantity"].Value);

            string action = isIncrease ? "TANG" : "GIAM";

            var dlg = new Form();
            dlg.Text = string.Format("{0} stock: {1}", action, productName);
            dlg.Size = new Size(380, 220);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            var lblCurrent = new Label();
            lblCurrent.Text = string.Format("Ton kho hien tai: {0}", currentQty);
            lblCurrent.Font = new Font("Segoe UI", 10);
            lblCurrent.Location = new Point(20, 20);
            lblCurrent.AutoSize = true;

            var lblAmount = new Label();
            lblAmount.Text = string.Format("So luong can {0}:", action.ToLower());
            lblAmount.Font = new Font("Segoe UI", 9);
            lblAmount.Location = new Point(20, 58);
            lblAmount.AutoSize = true;

            var txtAmount = new TextBox();
            txtAmount.Font = new Font("Segoe UI", 11);
            txtAmount.Size = new Size(100, 32);
            txtAmount.Location = new Point(200, 54);
            txtAmount.BorderStyle = BorderStyle.FixedSingle;
            txtAmount.Text = "1";

            var lblNote = new Label();
            lblNote.Text = "Ghi chu:";
            lblNote.Font = new Font("Segoe UI", 9);
            lblNote.Location = new Point(20, 100);
            lblNote.AutoSize = true;

            var txtNote = new TextBox();
            txtNote.Font = new Font("Segoe UI", 9);
            txtNote.Size = new Size(300, 28);
            txtNote.Location = new Point(20, 120);
            txtNote.BorderStyle = BorderStyle.FixedSingle;

            var btnConfirm = new Button();
            btnConfirm.Text = "XAC NHAN";
            btnConfirm.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnConfirm.Size = new Size(330, 38);
            btnConfirm.Location = new Point(20, 158);
            btnConfirm.BackColor = isIncrease
                                   ? Color.FromArgb(30, 130, 200)
                                   : Color.FromArgb(200, 100, 30);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Cursor = Cursors.Hand;

            btnConfirm.Click += (s, e) =>
            {
                if (!int.TryParse(txtAmount.Text, out int amount)
                    || amount <= 0)
                {
                    MessageBox.Show("So luong khong hop le!", "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!isIncrease && amount > currentQty)
                {
                    MessageBox.Show(
                        string.Format("Khong the giam qua ton kho ({0})!",
                            currentQty),
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    int change = isIncrease ? amount : -amount;
                    int qtyAfter = currentQty + change;
                    string type = isIncrease ? "IMPORT" : "EXPORT";
                    string note = string.IsNullOrWhiteSpace(txtNote.Text)
                                     ? string.Format("Admin dieu chinh stock ({0})",
                                         action)
                                     : txtNote.Text.Trim();

                    _productRepo.UpdateStock(productId, change);
                    _productRepo.LogStockMovement(
                        productId, 1, type, change, qtyAfter, note);

                    MessageBox.Show(
                        string.Format("Da {0} {1} don vi.\nTon kho moi: {2}",
                            action.ToLower(), amount, qtyAfter),
                        "Thanh cong",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                lblCurrent, lblAmount, txtAmount,
                lblNote, txtNote, btnConfirm
            });
            dlg.ShowDialog();
        }

        // ===== XÓA SẢN PHẨM =====
        private void DeleteProduct()
        {
            if (_dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui long chon san pham can xoa!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["Id"].Value);
            string name = _dgv.SelectedRows[0].Cells["Name"].Value.ToString();

            var confirm = MessageBox.Show(
                string.Format("Ban co chac muon xoa [{0}]?", name),
                "Xac nhan xoa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                _productRepo.Delete(id);
                MessageBox.Show("Da xoa san pham!", "OK",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}