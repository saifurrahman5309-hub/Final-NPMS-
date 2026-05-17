using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;
using PensionMgmt.Session;

namespace PensionMgmt.Forms
{
    /// <summary>
    /// My Payouts panel — visible to Pension Holders only.
    /// Shows all payout records that belong to the logged-in user's employee ID.
    /// </summary>
    public class MyPayoutsPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();
        private DataGridView dgv;
        private Label lblTotal;

        public MyPayoutsPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadMyPayouts();
        }

        private void BuildUI()
        {
            Label lHeader = new Label
            {
                Text      = "My Payout Records",
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds    = new Rectangle(20, 16, 500, 34)
            };
            this.Controls.Add(lHeader);

            Label lSub = new Label
            {
                Text      = "All pension payouts processed for your account.",
                ForeColor = Color.Gray,
                Bounds    = new Rectangle(20, 52, 500, 22)
            };
            this.Controls.Add(lSub);

            lblTotal = new Label
            {
                Text      = "",
                ForeColor = Color.FromArgb(25, 45, 80),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Bounds    = new Rectangle(20, 78, 500, 22)
            };
            this.Controls.Add(lblTotal);

            dgv = new DataGridView
            {
                Bounds              = new Rectangle(20, 108, this.Width - 40, this.Height - 130),
                Anchor              = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly            = true,
                AllowUserToAddRows  = false,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor     = Color.White,
                BorderStyle         = BorderStyle.None,
                RowHeadersVisible   = false,
                MultiSelect         = false
            };
            this.Controls.Add(dgv);
        }

        private void LoadMyPayouts()
        {
            // Match by the current user's employee ID (username is the employee ID)
            string myId = CurrentUser.Username;

            // Also check PensionHolder table for a linked employee ID
            object linkedId = _db.ExecuteScalar(
                "SELECT EmployeeId FROM PensionHolder WHERE UserName = @u",
                new System.Data.SqlClient.SqlParameter("@u", myId));

            string empId = (linkedId != null && linkedId != System.DBNull.Value &&
                            !string.IsNullOrEmpty(linkedId.ToString()))
                ? linkedId.ToString()
                : myId;

            DataTable dt = _db.GetTable(
                "SELECT MonthlyPension, LumpSum, ProcessedDate, Notes " +
                "FROM Payouts WHERE EmployeeId = @id ORDER BY ProcessedDate DESC",
                new System.Data.SqlClient.SqlParameter("@id", empId));

            dgv.DataSource = dt;

            if (dt.Rows.Count == 0)
                lblTotal.Text = "No payout records found for your account.";
            else
                lblTotal.Text = string.Format("{0} payout record(s) found.", dt.Rows.Count);
        }
    }
}
