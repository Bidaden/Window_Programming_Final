using System;
using System.Drawing;
using System.Windows.Forms;
using MySellerApp.DAL;
using MySellerApp.Models;

namespace MySellerApp.Forms
{
    public partial class FormSuppliers : Form
    {
        private readonly SupplierRepository _repo
            = new SupplierRepository();
        private DataGridView _dgv;

        public FormSuppliers()
        {
            InitializeComponent();
            SetupUI();
            LoadData();
        }

        private void SetupUI()
        {
            this.Text = "Quan ly nha cung cap";
            this.Size = new Size(800, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var pnlHeader = new Panel();
            pnlHeader.BackColor = Color.FromArgb(30, 80, 160);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 50;

            var lblTitle = new Label();
            lblTitle.Text = "QUAN LY NHA CUNG CAP";
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
            btnAdd.Text = "Them NCC moi";
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
            btnDelete.Text = "Xoa NCC";
            btnDelete.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnDelete.Size = new Size(120, 34);
            btnDelete.Location = new Point(172, 7);
            btnDelete.BackColor = Color.FromArgb(180, 30, 30);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.Cursor = Cursors.Hand;
            btnDelete.Click += (s, e) => DeleteSupplier();

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
            dlg.Text = "Them nha cung cap moi";
            dlg.Size = new Size(440, 320);
            dlg.StartPosition = FormStartPosition.CenterParent;
            dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
            dlg.MaximizeBox = false;
            dlg.BackColor = Color.White;

            int lx = 20, tx = 130, tw = 280, y = 20, gap = 48;

            Action<string, int> addLbl = (text, posY) =>
            {
                var l = new Label();
                l.Text = text;
                l.Font = new Font("Segoe UI", 9);
                l.Location = new Point(lx, posY + 4);
                l.AutoSize = true;
                dlg.Controls.Add(l);
            };

            addLbl("Ten NCC:", y);
            var txtName = new TextBox();
            txtName.Font = new Font("Segoe UI", 10);
            txtName.Size = new Size(tw, 30);
            txtName.Location = new Point(tx, y);
            txtName.BorderStyle = BorderStyle.FixedSingle;

            addLbl("So DT:", y + gap);
            var txtPhone = new TextBox();
            txtPhone.Font = new Font("Segoe UI", 10);
            txtPhone.Size = new Size(tw, 30);
            txtPhone.Location = new Point(tx, y + gap);
            txtPhone.BorderStyle = BorderStyle.FixedSingle;

            addLbl("Email:", y + gap * 2);
            var txtEmail = new TextBox();
            txtEmail.Font = new Font("Segoe UI", 10);
            txtEmail.Size = new Size(tw, 30);
            txtEmail.Location = new Point(tx, y + gap * 2);
            txtEmail.BorderStyle = BorderStyle.FixedSingle;

            addLbl("Dia chi:", y + gap * 3);
            var txtAddress = new TextBox();
            txtAddress.Font = new Font("Segoe UI", 10);
            txtAddress.Size = new Size(tw, 30);
            txtAddress.Location = new Point(tx, y + gap * 3);
            txtAddress.BorderStyle = BorderStyle.FixedSingle;

            var btnSave = new Button();
            btnSave.Text = "LUU";
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.Size = new Size(390, 38);
            btnSave.Location = new Point(lx, y + gap * 4);
            btnSave.BackColor = Color.FromArgb(30, 160, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Cursor = Cursors.Hand;

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Vui long nhap ten NCC!", "Loi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                try
                {
                    _repo.Add(new Supplier
                    {
                        Name = txtName.Text.Trim(),
                        Phone = txtPhone.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Address = txtAddress.Text.Trim()
                    });
                    MessageBox.Show("Them NCC thanh cong!", "OK",
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
                txtName, txtPhone, txtEmail, txtAddress, btnSave
            });
            dlg.ShowDialog();
        }

        private void DeleteSupplier()
        {
            if (_dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui long chon NCC can xoa!", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(_dgv.SelectedRows[0].Cells["Id"].Value);
            string name = _dgv.SelectedRows[0].Cells["Name"].Value.ToString();

            var confirm = MessageBox.Show(
                string.Format("Xoa nha cung cap [{0}]?", name),
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