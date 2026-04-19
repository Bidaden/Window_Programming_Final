using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormCategories : Form
    {
        private readonly CategoryRepository _repo
            = new CategoryRepository();
        private DataGridView _dgv;

        public FormCategories()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Quan ly danh muc";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "QUAN LY DANH MUC";
            lblTitle.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            pnlHeader.Controls.Add(lblTitle);

            var pnlToolbar = new Panel();
            pnlToolbar.BackColor = Color.FromArgb(245, 248, 255);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Height = 48;

            var btnAdd = new Button();
            btnAdd.Text = "Them danh muc";
            btnAdd.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnAdd.Size = new Size(150, 34);
            btnAdd.Location = new Point(12, 7);
            btnAdd.BackColor = Color.FromArgb(30, 160, 80);
            btnAdd.ForeColor = Color.White;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Cursor = Cursors.Hand;
            btnAdd.Click += (s, e) => ShowAddDialog();

            var btnDelete = new Button();
            btnDelete.Text = "Xoa danh muc";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Size = new Size(150, 34);
            btnDelete.Location = new Point(172, 7);
            btnDelete.BackColor = Color.FromArgb(180, 30, 30);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += (s, e) => DeleteCategory();

            pnlToolbar.Controls.AddRange(new Control[] { btnAdd, btnDelete });

            _dgv = new DataGridView();
            _dgv.Dock = DockStyle.Fill;
            _dgv.ReadOnly = true;
            _dgv.AllowUserToAddRows = false;
            _dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _dgv.BackgroundColor = Color.White;
            _dgv.RowTemplate.Height = 34;

            this.Controls.Add(_dgv);
            this.Controls.Add(pnlToolbar);
            this.Controls.Add(pnlHeader);
        }

        private void LoadData()
        {
            try { _dgv.DataSource = _repo.GetAll(); }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Loi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowAddDialog()
        {
            var dlg = new Form();
            dlg.Text = "Them danh muc moi";
            dlg.Size = new Size(400, 220);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            var lblName = new Label();
            lblName.Text = "Ten danh muc:";
            lblName.Font = new Font("Segoe UI", 9);
            lblName.Location = new Point(20, 20);
            lblName.AutoSize = true;

            var txtName = new TextBox();
            txtName.Font = new Font("Segoe UI", 10);
            txtName.Size = new Size(340, 30);
            txtName.Location = new Point(20, 42);
            txtName.BorderStyle = BorderStyle.FixedSingle;

            var lblDesc = new Label();
            lblDesc.Text = "Mo ta:";
            lblDesc.Font = new Font("Segoe UI", 9);
            lblDesc.Location = new Point(20, 82);
            lblDesc.AutoSize = true;

            var txtDesc = new TextBox();
            txtDesc.Font = new Font("Segoe UI", 10);
            txtDesc.Size = new Size(340, 30);
            txtDesc.Location = new Point(20, 104);
            txtDesc.BorderStyle = BorderStyle.FixedSingle;

            var btnSave = new Button();
            btnSave.Text = "LUU";
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Size = new Size(340, 38);
            btnSave.Location = new Point(20, 144);
            btnSave.BackColor = Color.FromArgb(30, 160, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Vui long nhap ten danh muc!", "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    _repo.Add(new Category
                    {
                        Name = txtName.Text.Trim(),
                        Description = txtDesc.Text.Trim()
                    });
                    MessageBox.Show("Them danh muc thanh cong!", "OK",
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
                lblName, txtName, lblDesc, txtDesc, btnSave
            });
            dlg.ShowDialog();
        }

        private void DeleteCategory()
        {
            if (_dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui long chon danh muc can xoa!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["Id"].Value);
            string name = _dgv.SelectedRows[0].Cells["Name"].Value.ToString();

            var confirm = MessageBox.Show(
                string.Format("Xoa danh muc [{0}]?", name),
                "Xac nhan", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            try
            {
                _repo.Delete(id);
                MessageBox.Show("Da xoa!", "OK",
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