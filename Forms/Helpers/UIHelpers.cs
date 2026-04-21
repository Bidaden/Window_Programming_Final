using System;
using System.Drawing;
using System.Windows.Forms;

namespace MySellerApp.Forms.Helpers
{
    public static class UIHelper
    {
        public static Panel CreateHeader(string title, Color bgColor)
        {
            var pnl = new Panel { BackColor = bgColor, Dock = DockStyle.Top, Height = 50 };
            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnl.Controls.Add(lbl);
            return pnl;
        }

        public static Button CreateBtn(string text, Color bg, EventHandler onClick, int x, int y, int w = 150, int h = 34)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(w, h),
                Location = new Point(x, y),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                Cursor = Cursors.Hand
            };
            btn.Click += onClick;
            return btn;
        }

        public static DataGridView CreateGrid(bool readOnly = true)
        {
            return new DataGridView
            {
                ReadOnly = readOnly,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowTemplate = { Height = 34 },
                ColumnHeadersDefaultCellStyle = { Font = new Font("Segoe UI", 9, FontStyle.Bold) }
            };
        }
    }
}