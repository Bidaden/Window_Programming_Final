namespace MySellerApp.Forms
{
    partial class FormCart
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(750, 650);
            this.Name = "FormCart";
            this.ResumeLayout(false);
        }
    }
}