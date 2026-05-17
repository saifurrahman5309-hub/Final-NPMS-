using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;
using PensionMgmt.Session;

namespace PensionMgmt.Forms
{
    public class DashboardPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();

        public DashboardPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadStats();
        }

        private Label MakeStatCard(string title, string value, Color accent, int x, int y)
        {
            Panel card = new Panel
            {
                Bounds    = new Rectangle(x, y, 200, 90),
                BackColor = Color.White
            };

            Label lTitle = new Label
            {
                Text      = title,
                ForeColor = Color.Gray,
                Font      = new Font("Segoe UI", 9f),
                Bounds    = new Rectangle(12, 10, 176, 20)
            };

            Label lValue = new Label
            {
                Text      = value,
                ForeColor = accent,
                Font      = new Font("Segoe UI", 22f, FontStyle.Bold),
                Bounds    = new Rectangle(12, 32, 176, 42),
                Name      = "val_" + title
            };

            card.Controls.Add(lTitle);
            card.Controls.Add(lValue);
            this.Controls.Add(card);
            return lValue;
        }

        private Label _lblTotalEmp, _lblActive, _lblRetiring, _lblPayouts;

        private void BuildUI()
        {
            Label lHeader = new Label
            {
                Text      = "Dashboard",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds    = new Rectangle(30, 20, 400, 36)
            };

            string roleDisplay = CurrentUser.Role.ToString()
                .Replace("PensionAdmin",   "Pension Admin")
                .Replace("PensionManager", "Pension Manager")
                .Replace("SystemAdmin",    "System Admin")
                .Replace("PensionHolder",  "Pension Holder");

            string welcomeName = string.IsNullOrEmpty(CurrentUser.FullName)
                ? CurrentUser.Username
                : CurrentUser.FullName;

            Label lWelcome = new Label
            {
                Text      = "Welcome, " + welcomeName + "  |  Role: " + roleDisplay,
                ForeColor = Color.Gray,
                Bounds    = new Rectangle(30, 56, 600, 22)
            };

            this.Controls.Add(lHeader);
            this.Controls.Add(lWelcome);

            if (CurrentUser.Role == UserRole.PensionHolder)
            {
                // Pension Holder sees a personal summary instead of system-wide stats
                Label lInfo = new Label
                {
                    Text      = "Use the sidebar to view your pension information, calculate your pension, or check your payout history.",
                    ForeColor = Color.DimGray,
                    AutoSize  = false,
                    Bounds    = new Rectangle(30, 100, 600, 44),
                    TextAlign = ContentAlignment.TopLeft
                };
                this.Controls.Add(lInfo);

                Panel tipCard = new Panel { BackColor = Color.White, Bounds = new Rectangle(30, 160, 420, 100) };
                Label lTip = new Label
                {
                    Text      = "Quick Tips:\n" +
                                "  - My Pension Info:  see your linked employee record and pension estimate.\n" +
                                "  - Pension Calc:     manually calculate pension for any scenario.\n" +
                                "  - My Payouts:       view all processed payout records for your account.",
                    ForeColor = Color.DimGray,
                    AutoSize  = false,
                    Bounds    = new Rectangle(12, 10, 396, 80)
                };
                tipCard.Controls.Add(lTip);
                this.Controls.Add(tipCard);
            }
            else
            {
                // System-wide stats for admin/manager roles
                _lblTotalEmp = MakeStatCard("Total Employees",    "...", Color.FromArgb(25, 45, 80),  30,  100);
                _lblActive   = MakeStatCard("Active Employees",   "...", Color.Green,                 250, 100);
                _lblRetiring = MakeStatCard("Retiring This Year", "...", Color.OrangeRed,             470, 100);
                _lblPayouts  = MakeStatCard("Total Payouts",      "...", Color.MediumVioletRed,       690, 100);
            }
        }

        private void LoadStats()
        {
            if (CurrentUser.Role == UserRole.PensionHolder)
                return; // no system stats for pension holders

            try
            {
                object total = _db.ExecuteScalar("SELECT COUNT(*) FROM Employees");
                _lblTotalEmp.Text = total != null ? total.ToString() : "0";

                object active = _db.ExecuteScalar("SELECT COUNT(*) FROM Employees WHERE Status='Active'");
                _lblActive.Text = active != null ? active.ToString() : "0";

                object retiring = _db.ExecuteScalar(
                      "SELECT COUNT(*) FROM Employees " +
                       "WHERE Status = 'Active' " +
                        // Completes 30 service years sometime this year
                       "AND YEAR(DATEADD(YEAR,30,JoiningDate)) = YEAR(GETDATE()) " +
                        // AND reaches age 59 this year or already has
                        "AND YEAR(DATEADD(YEAR,59,DateOfBirth)) <= YEAR(GETDATE())");
                _lblRetiring.Text = retiring != null ? retiring.ToString() : "0";

                object payouts = _db.ExecuteScalar("SELECT COUNT(*) FROM Payouts");
                _lblPayouts.Text = payouts != null ? payouts.ToString() : "0";
            }
            catch { /* silently ignore if tables don't exist yet */ }
        }
    }
}
