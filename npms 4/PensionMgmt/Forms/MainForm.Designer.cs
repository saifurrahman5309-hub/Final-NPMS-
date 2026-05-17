namespace PensionMgmt.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlSidebar  = new System.Windows.Forms.Panel();
            this.lblAppTitle = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblRoleName = new System.Windows.Forms.Label();
            this.pnlNav      = new System.Windows.Forms.Panel();
            this.pnlContent  = new System.Windows.Forms.Panel();
            this.pnlSidebar.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text            = "Govt. Pension Management System";
            this.WindowState     = System.Windows.Forms.FormWindowState.Maximized;
            this.MinimumSize     = new System.Drawing.Size(900, 600);
            this.StartPosition   = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Font            = new System.Drawing.Font("Segoe UI", 9.5f);
            this.Load           += new System.EventHandler(this.MainForm_Load);

            // pnlSidebar
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(25, 45, 80);
            this.pnlSidebar.Dock      = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Width     = 200;

            // lblAppTitle
            this.lblAppTitle.Text      = "BCS Pension";
            this.lblAppTitle.Font      = new System.Drawing.Font("Segoe UI", 13f, System.Drawing.FontStyle.Bold);
            this.lblAppTitle.ForeColor = System.Drawing.Color.White;
            this.lblAppTitle.AutoSize  = false;
            this.lblAppTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblAppTitle.Bounds    = new System.Drawing.Rectangle(0, 10, 200, 36);

            // lblUserName
            this.lblUserName.Text      = "";
            this.lblUserName.Font      = new System.Drawing.Font("Segoe UI", 8.5f, System.Drawing.FontStyle.Bold);
            this.lblUserName.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblUserName.AutoSize  = false;
            this.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblUserName.Bounds    = new System.Drawing.Rectangle(0, 48, 200, 20);

            // lblRoleName
            this.lblRoleName.Text      = "";
            this.lblRoleName.ForeColor = System.Drawing.Color.Silver;
            this.lblRoleName.AutoSize  = false;
            this.lblRoleName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblRoleName.Bounds    = new System.Drawing.Rectangle(0, 66, 200, 18);

            // pnlNav - navigation buttons go here
            this.pnlNav.Bounds = new System.Drawing.Rectangle(0, 92, 200, 460);
            this.pnlNav.Anchor = System.Windows.Forms.AnchorStyles.Top
                               | System.Windows.Forms.AnchorStyles.Bottom
                               | System.Windows.Forms.AnchorStyles.Left;

            this.pnlSidebar.Controls.Add(this.lblAppTitle);
            this.pnlSidebar.Controls.Add(this.lblUserName);
            this.pnlSidebar.Controls.Add(this.lblRoleName);
            this.pnlSidebar.Controls.Add(this.pnlNav);

            // pnlContent
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
            this.pnlContent.Dock      = System.Windows.Forms.DockStyle.Fill;

            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);

            this.pnlSidebar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel  pnlSidebar;
        private System.Windows.Forms.Label  lblAppTitle;
        private System.Windows.Forms.Label  lblUserName;
        private System.Windows.Forms.Label  lblRoleName;
        private System.Windows.Forms.Panel  pnlNav;
        private System.Windows.Forms.Panel  pnlContent;
    }
}
