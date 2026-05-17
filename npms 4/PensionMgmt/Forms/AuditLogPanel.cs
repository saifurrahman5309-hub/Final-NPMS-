using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;

namespace PensionMgmt.Forms
{
    /// <summary>
    /// Audit Log panel — visible to System Admin only.
    /// Shows a full history of every significant action performed in the system.
    /// </summary>
    public class AuditLogPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();
        private DataGridView dgv;
        private ComboBox cmbFilter;
        private TextBox txtSearch;
        private Label lblCount;

        public AuditLogPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadGrid();
        }

        private void BuildUI()
        {
            Label lHead = new Label
            {
                Text = "Audit Log",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(20, 16, 400, 34)
            };
            this.Controls.Add(lHead);

            Label lSub = new Label
            {
                Text = "Complete history of all system actions. Visible to System Admin only.",
                ForeColor = Color.Gray,
                Bounds = new Rectangle(20, 52, 600, 22)
            };
            this.Controls.Add(lSub);

            // Filter by action type
            Label lFilter = new Label
            {
                Text = "Filter:",
                Bounds = new Rectangle(20, 86, 50, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lFilter);

            cmbFilter = new ComboBox
            {
                Bounds = new Rectangle(72, 86, 200, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new[]
            {
                "All Actions",
                "Employee Added",
                "Employee Updated",
                "Employee Deleted",
                "User Approved",
                "User Rejected",
                "User Added Manually",
                "User Deleted",
                "Payout Pending",
                "Payout Approved",
                "Payout Rejected",
                "Login"
            });
            cmbFilter.SelectedIndex = 0;
            cmbFilter.SelectedIndexChanged += (s, e) => LoadGrid();
            this.Controls.Add(cmbFilter);

            // Search by employee ID or name
            Label lSearch = new Label
            {
                Text = "Search:",
                Bounds = new Rectangle(290, 86, 60, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lSearch);

            txtSearch = new TextBox
            {
                Bounds = new Rectangle(352, 86, 200, 26)
            };
            txtSearch.TextChanged += (s, e) => LoadGrid();
            this.Controls.Add(txtSearch);

            // Refresh button
            Button btnRefresh = new Button
            {
                Text = "Refresh",
                Bounds = new Rectangle(566, 84, 90, 28),
                BackColor = Color.FromArgb(25, 45, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.Click += (s, e) => LoadGrid();
            this.Controls.Add(btnRefresh);

            // Record count label
            lblCount = new Label
            {
                Text = "",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                Bounds = new Rectangle(20, 118, 400, 20)
            };
            this.Controls.Add(lblCount);

            // Grid
            dgv = new DataGridView
            {
                Bounds = new Rectangle(20, 142, this.Width - 40, this.Height - 162),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                MultiSelect = false
            };
            dgv.DataBindingComplete += Dgv_DataBindingComplete;
            this.Controls.Add(dgv);
        }

        private void LoadGrid()
        {
            string filter = cmbFilter.SelectedItem?.ToString() ?? "All Actions";
            string search = txtSearch.Text.Trim();

            string sql =
                "SELECT Id, ActionType, TargetId, TargetName, Description, PerformedBy, PerformedAt " +
                "FROM AuditLog WHERE 1=1";

            var parameters = new System.Collections.Generic.List<SqlParameter>();

            if (filter != "All Actions")
            {
                sql += " AND ActionType = @action";
                parameters.Add(new SqlParameter("@action", filter));
            }

            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND (TargetId LIKE @s OR TargetName LIKE @s OR PerformedBy LIKE @s OR Description LIKE @s)";
                parameters.Add(new SqlParameter("@s", "%" + search + "%"));
            }

            sql += " ORDER BY PerformedAt DESC";

            DataTable dt = _db.GetTable(sql, parameters.ToArray());
            dgv.DataSource = dt;

            lblCount.Text = string.Format("{0} record(s) found.", dt.Rows.Count);
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Color-code rows by action type for quick visual scanning
            foreach (DataGridViewRow row in dgv.Rows)
            {
                string action = row.Cells["ActionType"].Value?.ToString() ?? "";

                if (action == "Payout Approved" || action == "User Approved")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 255, 230);
                else if (action == "Payout Rejected" || action == "User Rejected" || action == "Employee Deleted" || action == "User Deleted")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                else if (action == "Payout Pending" || action == "Employee Added" || action == "User Added Manually")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 240, 255);
                else if (action == "Login")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(250, 250, 220);
            }
        }
    }
}