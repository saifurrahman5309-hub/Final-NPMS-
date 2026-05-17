namespace PensionMgmt.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.pnlCard = new System.Windows.Forms.Panel();
            this.lblUsernameL = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsernameError = new System.Windows.Forms.Label();
            this.lblPasswordL = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPasswordError = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.lblOrRegister = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.btnForgotPassword = new System.Windows.Forms.Button();
            this.pnlCard.SuspendLayout();
            this.SuspendLayout();

            // Form
            this.Text = "Govt. Pension Management - Login";
            this.Size = new System.Drawing.Size(460, 510);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = System.Drawing.Color.FromArgb(30, 50, 90);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5f);

            // lblTitle
            this.lblTitle.Text = "Government Pension Management";
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14f, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.AutoSize = false;
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Bounds = new System.Drawing.Rectangle(20, 20, 400, 36);

            // lblSubtitle
            this.lblSubtitle.Text = "Bangladesh Civil Service Portal";
            this.lblSubtitle.ForeColor = System.Drawing.Color.LightSkyBlue;
            this.lblSubtitle.AutoSize = false;
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSubtitle.Bounds = new System.Drawing.Rectangle(20, 56, 400, 22);

            // pnlCard
            this.pnlCard.BackColor = System.Drawing.Color.White;
            this.pnlCard.Bounds = new System.Drawing.Rectangle(50, 92, 340, 360);

            // lblUsernameL         y=20
            this.lblUsernameL.Text = "Employee ID (10 digits)";
            this.lblUsernameL.Bounds = new System.Drawing.Rectangle(20, 20, 300, 20);

            // txtUsername          y=44
            this.txtUsername.Bounds = new System.Drawing.Rectangle(20, 44, 300, 26);
            this.txtUsername.MaxLength = 10;

            // lblUsernameError     y=72
            this.lblUsernameError.Text = string.Empty;
            this.lblUsernameError.ForeColor = System.Drawing.Color.Crimson;
            this.lblUsernameError.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lblUsernameError.AutoSize = false;
            this.lblUsernameError.Bounds = new System.Drawing.Rectangle(20, 72, 300, 18);
            this.lblUsernameError.Visible = false;

            // lblPasswordL         y=98
            this.lblPasswordL.Text = "Password (6 digits)";
            this.lblPasswordL.Bounds = new System.Drawing.Rectangle(20, 98, 300, 20);

            // txtPassword          y=120
            this.txtPassword.Bounds = new System.Drawing.Rectangle(20, 120, 300, 26);
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.MaxLength = 6;

            // lblPasswordError     y=148
            this.lblPasswordError.Text = string.Empty;
            this.lblPasswordError.ForeColor = System.Drawing.Color.Crimson;
            this.lblPasswordError.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            this.lblPasswordError.AutoSize = false;
            this.lblPasswordError.Bounds = new System.Drawing.Rectangle(20, 148, 300, 18);
            this.lblPasswordError.Visible = false;

            // btnLogin             y=174
            this.btnLogin.Text = "LOG IN";
            this.btnLogin.Bounds = new System.Drawing.Rectangle(20, 174, 300, 36);
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(30, 50, 90);
            this.btnLogin.ForeColor = System.Drawing.Color.White;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);

            // lblOrRegister        y=222
            this.lblOrRegister.Text = "-------- New user? --------";
            this.lblOrRegister.ForeColor = System.Drawing.Color.Gray;
            this.lblOrRegister.AutoSize = false;
            this.lblOrRegister.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOrRegister.Bounds = new System.Drawing.Rectangle(20, 222, 300, 20);

            // btnRegister          y=246
            this.btnRegister.Text = "REGISTER";
            this.btnRegister.Bounds = new System.Drawing.Rectangle(20, 246, 300, 34);
            this.btnRegister.BackColor = System.Drawing.Color.FromArgb(0, 130, 100);
            this.btnRegister.ForeColor = System.Drawing.Color.White;
            this.btnRegister.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRegister.Font = new System.Drawing.Font("Segoe UI", 10f, System.Drawing.FontStyle.Bold);
            this.btnRegister.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);

            // btnForgotPassword    y=292
            this.btnForgotPassword.Text = "Forgot Password?";
            this.btnForgotPassword.Bounds = new System.Drawing.Rectangle(90, 292, 160, 24);
            this.btnForgotPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnForgotPassword.FlatAppearance.BorderSize = 0;
            this.btnForgotPassword.BackColor = System.Drawing.Color.Transparent;
            this.btnForgotPassword.ForeColor = System.Drawing.Color.FromArgb(30, 50, 90);
            this.btnForgotPassword.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnForgotPassword.Click += new System.EventHandler(this.btnForgotPassword_Click);

            this.pnlCard.Controls.Add(this.lblUsernameL);
            this.pnlCard.Controls.Add(this.txtUsername);
            this.pnlCard.Controls.Add(this.lblUsernameError);
            this.pnlCard.Controls.Add(this.lblPasswordL);
            this.pnlCard.Controls.Add(this.txtPassword);
            this.pnlCard.Controls.Add(this.lblPasswordError);
            this.pnlCard.Controls.Add(this.btnLogin);
            this.pnlCard.Controls.Add(this.lblOrRegister);
            this.pnlCard.Controls.Add(this.btnRegister);
            this.pnlCard.Controls.Add(this.btnForgotPassword);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.pnlCard);

            this.AcceptButton = this.btnLogin;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.LoginForm_FormClosed);

            this.pnlCard.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Panel pnlCard;
        private System.Windows.Forms.Label lblUsernameL;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblUsernameError;
        private System.Windows.Forms.Label lblPasswordL;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPasswordError;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblOrRegister;
        private System.Windows.Forms.Button btnRegister;
        private System.Windows.Forms.Button btnForgotPassword;
    }
}