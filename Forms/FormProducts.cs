using MySellerApp.DAL;
using MySellerApp.Forms.Helpers;
using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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

            this.Controls.Add(UIHelper.CreateHeader("QUAN LY SAN PHAM", Color.FromArgb(30, 80, 160)));

            var pnlToolbar = new Panel { BackColor = Color.FromArgb(245, 248, 255), Dock = DockStyle.Top, Height = 48 };
            pnlToolbar.Controls.AddRange(new Control[] {
        UIHelper.CreateBtn("Them san pham", Color.FromArgb(30, 160, 80), (s,e) => ShowAddProductDialog(), 12, 7, 150),
        UIHelper.CreateBtn("+ Tang stock", Color.FromArgb(30, 130, 200), (s,e) => AdjustStock(true), 172, 7, 130),
        UIHelper.CreateBtn("- Giam stock", Color.FromArgb(200, 100, 30), (s,e) => AdjustStock(false), 312, 7, 130),
        UIHelper.CreateBtn("Xoa san pham", Color.FromArgb(180, 30, 30), (s,e) => DeleteProduct(), 452, 7, 130),
        UIHelper.CreateBtn("Lam moi", Color.FromArgb(240, 240, 240), (s,e) => LoadData(), 592, 7, 100)
    });
            this.Controls.Add(pnlToolbar);

            _dgv = UIHelper.CreateGrid();
            _dgv.Dock = DockStyle.Fill;
            _dgv.MultiSelect = false;
            _dgv.RowPrePaint += (s, e) =>
            {
                if (e.RowIndex < 0 || e.RowIndex >= _dgv.Rows.Count) return;
                var row = _dgv.Rows[e.RowIndex];
                if (row.Cells["Quantity"]?.Value == null || row.Cells["MinStock"]?.Value == null) return;
                if (Convert.ToInt32(row.Cells["Quantity"].Value) <= Convert.ToInt32(row.Cells["MinStock"].Value))
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