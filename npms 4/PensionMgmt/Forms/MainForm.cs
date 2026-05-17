using System;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Session;

namespace PensionMgmt.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblUserName.Text = string.IsNullOrEmpty(CurrentUser.FullName)
                ? CurrentUser.Username
                : CurrentUser.FullName;

            string roleName = CurrentUser.Role.ToString()
                .Replace("PensionAdmin", "Pension Admin")
                .Replace("PensionManager", "Pension Manager")
                .Replace("SystemAdmin", "System Admin")
                .Replace("PensionHolder", "Pension Holder");
            lblRoleName.Text = roleName;

            BuildMenu();
            LoadPanel(new DashboardPanel());
        }

        private void BuildMenu()
        {
            pnlNav.Controls.Clear();
            int y = 0;

            AddNavButton("   Dashboard", ref y, () => LoadPanel(new DashboardPanel()));

            if (CurrentUser.Role == UserRole.SystemAdmin ||
                CurrentUser.Role == UserRole.PensionAdmin ||
                CurrentUser.Role == UserRole.PensionManager)
            {
                AddNavButton("   Employees", ref y, () => LoadPanel(new EmployeesPanel()));
                AddNavButton("   Payouts", ref y, () => LoadPanel(new PayoutsPanel()));
                AddNavButton("   Pension Calc", ref y, () => LoadPanel(new PensionCalcPanel()));
                AddNavButton("   Retirement", ref y, () => LoadPanel(new RetirementPanel()));
            }

            if (CurrentUser.Role == UserRole.SystemAdmin ||
                CurrentUser.Role == UserRole.PensionAdmin)
            {
                AddNavButton("   User Control", ref y, () => LoadPanel(new UserControlPanel()));
            }

            // Audit Log — System Admin only
            if (CurrentUser.Role == UserRole.SystemAdmin)
            {
                AddNavButton("   Audit Log", ref y, () => LoadPanel(new AuditLogPanel()));
            }

            if (CurrentUser.Role == UserRole.PensionHolder)
            {
                AddNavButton("   My Pension Info", ref y, () => LoadPanel(new PensionHolderPanel()));
                AddNavButton("   Pension Calc", ref y, () => LoadPanel(new PensionCalcPanel()));
                AddNavButton("   My Payouts", ref y, () => LoadPanel(new MyPayoutsPanel()));
            }

            AddNavButton("   Manage Account", ref y, () => LoadPanel(new ManageAccountPanel()));

            Button btnLogout = new Button
            {
                Text = "   Logout",
                Bounds = new Rectangle(0, y, pnlNav.Width, 46),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.FromArgb(200, 60, 60),
                Font = new Font("Segoe UI", 10f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                CurrentUser.Clear();
                new LoginForm().Show();
                this.Close();
            };
            pnlNav.Controls.Add(btnLogout);
        }

        private void AddNavButton(string text, ref int top, Action onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Bounds = new Rectangle(0, top, pnlNav.Width, 44),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 255, 255);
            btn.Click += (s, e) => onClick();
            pnlNav.Controls.Add(btn);
            top += 46;
        }

        private void LoadPanel(UserControl panel)
        {
            pnlContent.Controls.Clear();
            panel.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(panel);
        }
    }
}