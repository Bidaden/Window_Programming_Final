using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.BL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormCart : Form
    {
        private readonly SaleOrderService _saleService
            = new SaleOrderService();
        private readonly List<CartItem> _cartItems;

        private DataGridView _dgvCart;
        private Label _lblTotal;
        private TextBox _txtName;
        private TextBox _txtPhone;

        public FormCart(List<CartItem> cartItems)
        {
            InitializeComponent();
            _cartItems = cartItems;
            SetupUI();
            LoadCart();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Gio hang";
            this.Size = new Size(750, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // ===== HEADER =====
            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "GIO HANG CUA BAN";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            // ===== DATAGRIDVIEW =====
            _dgvCart = new DataGridView();
            _dgvCart.Size = new Size(710, 230);
            _dgvCart.Location = new Point(20, 65);
            _dgvCart.ReadOnly = false;
            _dgvCart.AllowUserToAddRows = false;
            _dgvCart.AllowUserToDeleteRows = false;
            _dgvCart.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgvCart.BackgroundColor = Color.White;
            _dgvCart.ColumnHeadersHeight = 36;
            _dgvCart.RowTemplate.Height = 36;
            _dgvCart.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var colName = new DataGridViewTextBoxColumn();
            colName.Name = "colName";
            colName.HeaderText = "San pham";
            colName.ReadOnly = true;
            colName.FillWeight = 40;

            var colPrice = new DataGridViewTextBoxColumn();
            colPrice.Name = "colPrice";
            colPrice.HeaderText = "Don gia";
            colPrice.ReadOnly = true;
            colPrice.FillWeight = 20;
            colPrice.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            var colQty = new DataGridViewTextBoxColumn();
            colQty.Name = "colQty";
            colQty.HeaderText = "So luong";
            colQty.ReadOnly = false;
            colQty.FillWeight = 15;
            colQty.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            var colSub = new DataGridViewTextBoxColumn();
            colSub.Name = "colSubTotal";
            colSub.HeaderText = "Thanh tien";
            colSub.ReadOnly = true;
            colSub.FillWeight = 20;
            colSub.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            var colDel = new DataGridViewButtonColumn();
            colDel.Name = "colRemove";
            colDel.HeaderText = "";
            colDel.Text = "Xoa";
            colDel.UseColumnTextForButtonValue = true;
            colDel.FillWeight = 5;

            _dgvCart.Columns.AddRange(new DataGridViewColumn[] {
                colName, colPrice, colQty, colSub, colDel
            });

            _dgvCart.CellValueChanged += DgvCart_CellValueChanged;
            _dgvCart.CellContentClick += DgvCart_CellContentClick;
            _dgvCart.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (_dgvCart.IsCurrentCellDirty)
                    _dgvCart.CommitEdit(
                        DataGridViewDataErrorContexts.Commit);
            };

            // ===== TỔNG TIỀN =====
            var pnlTotal = new Panel();
            pnlTotal.Size = new Size(710, 50);
            pnlTotal.Location = new Point(20, 305);
            pnlTotal.BackColor = Color.FromArgb(245, 248, 255);

            var lblTotalLabel = new Label();
            lblTotalLabel.Text = "TONG CONG:";
            lblTotalLabel.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblTotalLabel.ForeColor = Color.FromArgb(30, 60, 130);
            lblTotalLabel.Location = new Point(12, 12);
            lblTotalLabel.AutoSize = true;

            _lblTotal = new Label();
            _lblTotal.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            _lblTotal.ForeColor = Color.FromArgb(200, 50, 50);
            _lblTotal.Size = new Size(300, 34);
            _lblTotal.Location = new Point(400, 8);
            _lblTotal.TextAlign = ContentAlignment.MiddleRight;

            pnlTotal.Controls.AddRange(new Control[] {
                lblTotalLabel, _lblTotal
            });

            // ===== THÔNG TIN KHÁCH HÀNG =====
            var pnlCustomer = new Panel();
            pnlCustomer.Size = new Size(710, 140);
            pnlCustomer.Location = new Point(20, 365);
            pnlCustomer.BackColor = Color.FromArgb(250, 250, 252);

            var lblInfo = new Label();
            lblInfo.Text = "THONG TIN KHACH HANG";
            lblInfo.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblInfo.ForeColor = Color.FromArgb(30, 60, 130);
            lblInfo.Location = new Point(12, 12);
            lblInfo.AutoSize = true;

            var lblName = new Label();
            lblName.Text = "Ho ten:";
            lblName.Font = new Font("Segoe UI", 9);
            lblName.Location = new Point(12, 42);
            lblName.AutoSize = true;

            _txtName = new TextBox();
            _txtName.Font = new Font("Segoe UI", 10);
            _txtName.Size = new Size(580, 30);
            _txtName.Location = new Point(80, 38);
            _txtName.BorderStyle = BorderStyle.FixedSingle;

            var lblPhone = new Label();
            lblPhone.Text = "So DT:";
            lblPhone.Font = new Font("Segoe UI", 9);
            lblPhone.Location = new Point(12, 82);
            lblPhone.AutoSize = true;

            _txtPhone = new TextBox();
            _txtPhone.Font = new Font("Segoe UI", 10);
            _txtPhone.Size = new Size(580, 30);
            _txtPhone.Location = new Point(80, 78);
            _txtPhone.BorderStyle = BorderStyle.FixedSingle;

            pnlCustomer.Controls.AddRange(new Control[] {
                lblInfo, lblName, _txtName, lblPhone, _txtPhone
            });

            // ===== NÚT =====
            var btnContinue = new Button();
            btnContinue.Text = "Tiep tuc mua";
            btnContinue.Font = new Font("Segoe UI", 10);
            btnContinue.Size = new Size(160, 48);
            btnContinue.Location = new Point(20, 560);
            btnContinue.BackColor = Color.FromArgb(240, 240, 240);
            btnContinue.FlatStyle = FlatStyle.Flat;
            btnContinue.Cursor = Cursors.Hand;
            btnContinue.Click += (s, e) => this.Close();

            var btnCheckout = new Button();
            btnCheckout.Text = "XAC NHAN MUA HANG";
            btnCheckout.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnCheckout.Size = new Size(520, 48);
            btnCheckout.Location = new Point(190, 560);
            btnCheckout.BackColor = Color.FromArgb(30, 160, 80);
            btnCheckout.ForeColor = Color.White;
            btnCheckout.FlatStyle = FlatStyle.Flat;
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Cursor = Cursors.Hand;
            btnCheckout.Click += BtnCheckout_Click;

            this.Controls.AddRange(new Control[] {
                pnlHeader, _dgvCart, pnlTotal,
                pnlCustomer, btnContinue, btnCheckout
            });
        }

        private void LoadCart()
        {
            _dgvCart.Rows.Clear();
            foreach (var item in _cartItems)
            {
                _dgvCart.Rows.Add(
                    item.ProductName,
                    string.Format("{0:N0} d", item.UnitPrice),
                    item.Quantity,
                    string.Format("{0:N0} d", item.SubTotal)
                );
            }
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            decimal total = 0;
            foreach (var item in _cartItems)
                total += item.SubTotal;
            _lblTotal.Text = string.Format("{0:N0} d", total);
        }

        private void DgvCart_CellValueChanged(object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 2 || e.RowIndex < 0) return;

            var cell = _dgvCart.Rows[e.RowIndex].Cells[2];
            if (!int.TryParse(cell.Value?.ToString(), out int newQty)
                || newQty <= 0)
            {
                MessageBox.Show("So luong phai lon hon 0!", "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cell.Value = _cartItems[e.RowIndex].Quantity;
                return;
            }

            if (newQty > _cartItems[e.RowIndex].MaxStock)
            {
                MessageBox.Show(
                    string.Format("Chi con {0} san pham trong kho!",
                        _cartItems[e.RowIndex].MaxStock),
                    "Loi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cell.Value = _cartItems[e.RowIndex].Quantity;
                return;
            }

            _cartItems[e.RowIndex].Quantity = newQty;
            _dgvCart.Rows[e.RowIndex].Cells[3].Value =
                string.Format("{0:N0} d", _cartItems[e.RowIndex].SubTotal);
            UpdateTotal();
        }

        private void DgvCart_CellContentClick(object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 4 || e.RowIndex < 0) return;

            _cartItems.RemoveAt(e.RowIndex);
            LoadCart();

            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Gio hang trong!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void BtnCheckout_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = _saleService.PlaceOrder(
                    _txtName.Text.Trim(),
                    _txtPhone.Text.Trim(),
                    _cartItems);

                decimal total = 0;
                foreach (var item in _cartItems) total += item.SubTotal;

                var result = MessageBox.Show(
                    string.Format(
                        "Dat hang thanh cong!\n\n" +
                        "Ma don hang : #{0}\n" +
                        "Khach hang  : {1}\n" +
                        "So dien thoai: {2}\n" +
                        "Tong tien   : {3:N0} d\n\n" +
                        "Ban co muon xem lich su don hang khong?",
                        orderId,
                        _txtName.Text.Trim(),
                        _txtPhone.Text.Trim(),
                        total),
                    "Dat hang thanh cong!",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);

                _cartItems.Clear();

                if (result == DialogResult.Yes)
                    new FormOrderHistory().ShowDialog();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}