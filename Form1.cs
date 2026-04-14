using System;
using System.Windows.Forms;
using MySellerApp.Forms;

namespace MySellerApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Ẩn Form1, mở thẳng FormLogin
            this.Hide();
            new FormLogin().ShowDialog();
            this.Close();
        }
    }
}