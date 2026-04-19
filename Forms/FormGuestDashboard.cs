using MySellerApp.BL;
using MySellerApp.DAL;
using MySellerApp.DAL_EF;
using MySellerApp.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MySellerApp.Forms
{
    public partial class FormGuestDashboard : Form
    {
        private readonly ProductService _productService
    = new ProductService(RepositoryFactory.CreateProductRepository());
        private readonly CategoryRepository _categoryRepo
            = new CategoryRepository();

        private List<Product> _allProducts = new List<Product>();
        private List<CartItem> _cart = new List<CartItem>();

        private Panel _cardPanel;
        private TextBox _txtSearch;
        private ComboBox _cboCategory;
        private Label _lblCount;
        private Button _btnViewCart;

        public FormGuestDashboard()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "ElectronicShop - Xem san pham";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);
            this.MinimumSize = new Size(900, 600);

            // ===== HEADER =====
            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;

            var lblShop = new Label();
            lblShop.Text = "ELECTRONIC SHOP";
            lblShop.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblShop.ForeColor = Color.White;
            lblShop.Location = new Point(20, 14);
            lblShop.AutoSize = true;

            var lblSub = new Label();
            lblSub.Text = "Cua hang dien tu";
            lblSub.Font = new Font("Segoe UI", 9);
            lblSub.ForeColor = Color.FromArgb(200, 220, 255);
            lblSub.Location = new Point(22, 38);
            lblSub.AutoSize = true;

            var btnHistory = new Button();
            btnHistory.Text = "Lich su don hang";
            btnHistory.Font = new Font("Segoe UI", 9);
            btnHistory.Size = new Size(150, 30);
            btnHistory.Location = new Point(780, 15);
            btnHistory.BackColor = Color.FromArgb(50, 110, 200);
            btnHistory.ForeColor = Color.White;
            btnHistory.FlatStyle = FlatStyle.Flat;
            btnHistory.FlatAppearance.BorderSize = 0;
            btnHistory.Cursor = Cursors.Hand;
            btnHistory.Click += (s, e) =>
                new FormOrderHistory().ShowDialog();

            pnlHeader.Controls.AddRange(new Control[] {
                lblShop, lblSub, btnHistory
            });

            // ===== TOOLBAR =====
            var pnlToolbar = new Panel();
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 55;

            var lblSearch = new Label();
            lblSearch.Text = "Tim kiem:";
            lblSearch.Font = new Font("Segoe UI", 9);
            lblSearch.Location = new Point(15, 18);
            lblSearch.AutoSize = true;

            _txtSearch = new TextBox();
            _txtSearch.Font = new Font("Segoe UI", 10);
            _txtSearch.Size = new Size(230, 30);
            _txtSearch.Location = new Point(85, 14);
            _txtSearch.BorderStyle = BorderStyle.FixedSingle;

            var lblCat = new Label();
            lblCat.Text = "Danh muc:";
            lblCat.Font = new Font("Segoe UI", 9);
            lblCat.Location = new Point(330, 18);
            lblCat.AutoSize = true;

            _cboCategory = new ComboBox();
            _cboCategory.Font = new Font("Segoe UI", 10);
            _cboCategory.Size = new Size(180, 30);
            _cboCategory.Location = new Point(410, 14);
            _cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            var btnSearch = new Button();
            btnSearch.Text = "Tim";
            btnSearch.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnSearch.Size = new Size(70, 30);
            btnSearch.Location = new Point(600, 13);
            btnSearch.BackColor = Color.FromArgb(30, 80, 160);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.Cursor = Cursors.Hand;

            var btnReset = new Button();
            btnReset.Text = "Tat ca";
            btnReset.Font = new Font("Segoe UI", 9);
            btnReset.Size = new Size(70, 30);
            btnReset.Location = new Point(680, 13);
            btnReset.BackColor = Color.FromArgb(240, 240, 240);
            btnReset.FlatStyle = FlatStyle.Flat;
            btnReset.Cursor = Cursors.Hand;

            _lblCount = new Label();
            _lblCount.Font = new Font("Segoe UI", 9);
            _lblCount.ForeColor = Color.Gray;
            _lblCount.Location = new Point(760, 18);
            _lblCount.AutoSize = true;

            _btnViewCart = new Button();
            _btnViewCart.Text = "Gio hang (0)";
            _btnViewCart.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            _btnViewCart.Size = new Size(140, 30);
            _btnViewCart.Location = new Point(930, 13);
            _btnViewCart.BackColor = Color.FromArgb(30, 160, 80);
            _btnViewCart.ForeColor = Color.White;
            _btnViewCart.FlatStyle = FlatStyle.Flat;
            _btnViewCart.FlatAppearance.BorderSize = 0;
            _btnViewCart.Cursor = Cursors.Hand;

            pnlToolbar.Controls.AddRange(new Control[] {
                lblSearch, _txtSearch,
                lblCat, _cboCategory,
                btnSearch, btnReset,
                _lblCount, _btnViewCart
            });

            // ===== CARD PANEL =====
            _cardPanel = new Panel();
            _cardPanel.AutoScroll = true;
            _cardPanel.BackColor = Color.FromArgb(245, 245, 245);
            _cardPanel.Dock = DockStyle.Fill;

            // Thứ tự add quan trọng với Dock
            this.Controls.Add(_cardPanel);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlHeader);

            // ===== SỰ KIỆN =====
            btnSearch.Click += (s, e) => FilterProducts();
            btnReset.Click += (s, e) =>
            {
                _txtSearch.Text = "";
                _cboCategory.SelectedIndex = 0;
                RenderCards(_allProducts);
            };
            _txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) FilterProducts();
            };
            _cboCategory.SelectedIndexChanged += (s, e) => FilterProducts();
            _btnViewCart.Click += (s, e) =>
            {
                if (_cart.Count == 0)
                {
                    MessageBox.Show("Gio hang dang trong!", "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                new FormCart(_cart).ShowDialog();
                // Reload sau khi mua xong
                _cart.Clear();
                UpdateCartButton();
                LoadData();
            };
        }

        private void LoadData()
        {
            try
            {
                _allProducts = _productService.GetAllProducts();

                _cboCategory.Items.Clear();
                _cboCategory.Items.Add("-- Tat ca danh muc --");
                var cats = _categoryRepo.GetAll();
                foreach (var c in cats)
                    _cboCategory.Items.Add(c.Name);
                _cboCategory.SelectedIndex = 0;

                RenderCards(_allProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FilterProducts()
        {
            var keyword = _txtSearch.Text.Trim().ToLower();
            var category = _cboCategory.SelectedIndex > 0
                           ? _cboCategory.SelectedItem.ToString() : "";

            var filtered = _allProducts;

            if (!string.IsNullOrEmpty(keyword))
                filtered = filtered.FindAll(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.SKU.ToLower().Contains(keyword));

            if (!string.IsNullOrEmpty(category))
                filtered = filtered.FindAll(p => p.Category == category);

            RenderCards(filtered);
        }

        private void RenderCards(List<Product> products)
        {
            _cardPanel.Controls.Clear();
            _lblCount.Text = string.Format("Hien thi {0} san pham",
                products.Count);

            int cardW = 220;
            int cardH = 300;
            int gapX = 16;
            int gapY = 16;
            int startX = 12;
            int startY = 12;
            int maxCols = Math.Max(1,
                (_cardPanel.Width - startX) / (cardW + gapX));
            int col = 0;
            int row = 0;

            foreach (var p in products)
            {
                if (col >= maxCols) { col = 0; row++; }

                var card = CreateCard(p,
                    startX + col * (cardW + gapX),
                    startY + row * (cardH + gapY),
                    cardW, cardH);

                _cardPanel.Controls.Add(card);
                col++;
            }

            int totalRows = (int)Math.Ceiling(
                (double)products.Count / Math.Max(1, maxCols));
            _cardPanel.AutoScrollMinSize = new Size(0,
                startY + totalRows * (cardH + gapY) + 20);
        }

        private Panel CreateCard(Product p, int x, int y, int w, int h)
        {
            var card = new Panel();
            card.Size = new Size(w, h);
            card.Location = new Point(x, y);
            card.BackColor = Color.White;
            card.Cursor = Cursors.Default;

            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(220, 220, 220), 1))
                    e.Graphics.DrawRectangle(pen,
                        0, 0, card.Width - 1, card.Height - 1);
            };

            // ===== ẢNH SẢN PHẨM =====
            var picBox = new PictureBox();
            picBox.Size = new Size(w, 150);
            picBox.Location = new Point(0, 0);
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
            picBox.BackColor = Color.FromArgb(248, 248, 248);

            string imgPath = Path.Combine(
              Application.StartupPath,
              "Images", "Products", p.Name + ".jpg");

            if (File.Exists(imgPath))
            {
                picBox.Image = Image.FromFile(imgPath);
            }
            else
            {
                picBox.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(
                        new SolidBrush(Color.FromArgb(235, 240, 255)),
                        0, 0, picBox.Width, picBox.Height);
                    using (var f = new Font("Segoe UI", 9))
                    using (var b = new SolidBrush(Color.FromArgb(150, 150, 180)))
                    {
                        var sf = new StringFormat();
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(p.SKU, f, b,
                            new RectangleF(0, 0, picBox.Width, picBox.Height),
                            sf);
                    }
                };
            }

            // ===== CATEGORY BADGE =====
            var lblCat = new Label();
            lblCat.Text = p.Category;
            lblCat.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            lblCat.ForeColor = Color.FromArgb(30, 80, 160);
            lblCat.BackColor = Color.FromArgb(230, 238, 255);
            lblCat.Size = new Size(w - 16, 18);
            lblCat.Location = new Point(8, 156);
            lblCat.TextAlign = ContentAlignment.MiddleLeft;
            lblCat.Padding = new Padding(4, 0, 0, 0);

            // ===== TÊN SẢN PHẨM =====
            var lblName = new Label();
            lblName.Text = p.Name;
            lblName.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblName.ForeColor = Color.FromArgb(30, 30, 30);
            lblName.Size = new Size(w - 16, 38);
            lblName.Location = new Point(8, 178);
            lblName.TextAlign = ContentAlignment.TopLeft;

            // ===== GIÁ =====
            var lblPrice = new Label();
            lblPrice.Text = string.Format("{0:N0} d", p.Price);
            lblPrice.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblPrice.ForeColor = Color.FromArgb(200, 50, 50);
            lblPrice.Size = new Size(w - 16, 26);
            lblPrice.Location = new Point(8, 218);
            lblPrice.TextAlign = ContentAlignment.MiddleLeft;

            // ===== SỐ LƯỢNG =====
            var lblQty = new Label();
            lblQty.Text = p.Quantity > 0
                               ? string.Format("Con: {0} san pham", p.Quantity)
                               : "Het hang";
            lblQty.Font = new Font("Segoe UI", 8);
            lblQty.ForeColor = p.Quantity <= p.MinStock
                               ? Color.OrangeRed : Color.Gray;
            lblQty.Size = new Size(w - 16, 18);
            lblQty.Location = new Point(8, 246);
            lblQty.TextAlign = ContentAlignment.MiddleLeft;

            // ===== NÚT THÊM VÀO GIỎ =====
            var btnAdd = new Button();
            btnAdd.Text = p.Quantity > 0 ? "Them vao gio" : "Het hang";
            btnAdd.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            btnAdd.Size = new Size(w - 16, 28);
            btnAdd.Location = new Point(8, 266);
            btnAdd.BackColor = p.Quantity > 0
                               ? Color.FromArgb(30, 80, 160)
                               : Color.FromArgb(180, 180, 180);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = p.Quantity > 0
                               ? Cursors.Hand : Cursors.Default;
            btnAdd.Enabled = p.Quantity > 0;

            var captured = p;
            btnAdd.Click += (s, e) => AddToCart(captured);

            card.Controls.AddRange(new Control[] {
                picBox, lblCat, lblName,
                lblPrice, lblQty, btnAdd
            });

            // Hover effect
            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(250, 252, 255);
                card.Invalidate();
            };
            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
                card.Invalidate();
            };

            return card;
        }

        private void AddToCart(Product p)
        {
            var existing = _cart.Find(c => c.ProductId == p.Id);
            if (existing != null)
            {
                if (existing.Quantity >= p.Quantity)
                {
                    MessageBox.Show(
                        string.Format("Da dat toi da {0} san pham trong kho!",
                            p.Quantity),
                        "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                existing.Quantity++;
            }
            else
            {
                _cart.Add(new CartItem
                {
                    ProductId = p.Id,
                    SKU = p.SKU,
                    ProductName = p.Name,
                    UnitPrice = p.Price,
                    Quantity = 1,
                    MaxStock = p.Quantity
                });
            }

            UpdateCartButton();

            MessageBox.Show(
                string.Format("Da them [{0}] vao gio hang!", p.Name),
                "Thanh cong",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateCartButton()
        {
            int total = 0;
            foreach (var c in _cart) total += c.Quantity;
            _btnViewCart.Text = string.Format("Gio hang ({0})", total);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (_allProducts != null && _allProducts.Count > 0)
                RenderCards(_allProducts);
        }
    }
}