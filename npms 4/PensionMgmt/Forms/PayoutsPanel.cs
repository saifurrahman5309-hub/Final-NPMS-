using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;
using PensionMgmt.Services;

namespace PensionMgmt.Forms
{
    public class PayoutsPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();
        private DataGridView dgv;
        private TextBox txtEmpId, txtNotes;
        private Panel pnlForm;

        public PayoutsPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadGrid();
        }

        private void BuildUI()
        {
            Label lHead = new Label
            {
                Text = "Pension Payouts",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(20, 16, 400, 34)
            };
            this.Controls.Add(lHead);

            Button btnAdd = new Button
            {
                Text = "+ Process Payout",
                Bounds = new Rectangle(20, 58, 160, 30),
                BackColor = Color.FromArgb(25, 45, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += (s, e) => { pnlForm.Visible = true; txtEmpId.Clear(); txtNotes.Clear(); };
            this.Controls.Add(btnAdd);

            Button btnApprove = new Button
            {
                Text = "✔ Approve Selected",
                Bounds = new Rectangle(190, 58, 160, 30),
                BackColor = Color.FromArgb(0, 130, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Click += BtnApprove_Click;
            this.Controls.Add(btnApprove);

            Button btnReject = new Button
            {
                Text = "✘ Reject Selected",
                Bounds = new Rectangle(360, 58, 155, 30),
                BackColor = Color.Firebrick,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.Click += BtnReject_Click;
            this.Controls.Add(btnReject);

            dgv = new DataGridView
            {
                Bounds = new Rectangle(20, 100, this.Width - 40, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                MultiSelect = false
            };
            this.Controls.Add(dgv);

            pnlForm = new Panel
            {
                BackColor = Color.White,
                Bounds = new Rectangle(20, 410, 520, 200),
                Visible = false
            };

            Label lTitle = new Label { Text = "Process New Payout", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.FromArgb(25, 45, 80), Bounds = new Rectangle(10, 10, 300, 28) };
            Label lEmpId = new Label { Text = "Employee ID *", Bounds = new Rectangle(10, 48, 140, 26), TextAlign = ContentAlignment.MiddleLeft };
            txtEmpId = new TextBox { Bounds = new Rectangle(154, 48, 160, 26), MaxLength = 20 };
            Label lNotes = new Label { Text = "Notes", Bounds = new Rectangle(10, 86, 140, 26), TextAlign = ContentAlignment.MiddleLeft };
            txtNotes = new TextBox { Bounds = new Rectangle(154, 86, 340, 26) };
            Label lInfo = new Label { Text = "Payout will be saved as Pending and must be approved.", ForeColor = Color.DimGray, Bounds = new Rectangle(10, 116, 490, 20) };

            Button btnSave = new Button { Text = "💾 Save Payout", Bounds = new Rectangle(10, 142, 140, 30), BackColor = Color.FromArgb(25, 45, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            Button btnCancel = new Button { Text = "Cancel", Bounds = new Rectangle(160, 142, 80, 30), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => pnlForm.Visible = false;

            pnlForm.Controls.AddRange(new Control[] { lTitle, lEmpId, txtEmpId, lNotes, txtNotes, lInfo, btnSave, btnCancel });
            this.Controls.Add(pnlForm);
        }

        private void LoadGrid()
        {
            dgv.DataSource = _db.GetTable(
                "SELECT Id, EmployeeId, FullName, MonthlyPension, LumpSum, ProcessedDate, Status, Notes " +
                "FROM Payouts ORDER BY ProcessedDate DESC");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string empId = txtEmpId.Text.Trim();
            if (string.IsNullOrEmpty(empId)) { MessageBox.Show("Enter an Employee ID."); return; }

            DataTable dt = _db.GetTable(
                "SELECT FullName, BasicSalary, DateOfBirth, JoiningDate, Rank FROM Employees WHERE EmployeeId=@id",
                new SqlParameter("@id", empId));

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Employee not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = dt.Rows[0];
            string name = row["FullName"].ToString();
            decimal salary = Convert.ToDecimal(row["BasicSalary"]);
            DateTime dob = Convert.ToDateTime(row["DateOfBirth"]);
            DateTime doj = Convert.ToDateTime(row["JoiningDate"]);
            string rank = row["Rank"].ToString();

            bool ok = PensionCalculator.Calculate(salary, dob, doj, rank,
                out decimal monthly, out decimal lump, out string msg);

            if (!ok) { MessageBox.Show(msg, "Not Eligible", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            object existing = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Payouts WHERE EmployeeId = @id",
                new SqlParameter("@id", empId));
            if (Convert.ToInt32(existing) > 0)
            {
                MessageBox.Show("A payout record already exists for this employee.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int n = _db.Execute(
                "INSERT INTO Payouts (EmployeeId, FullName, MonthlyPension, LumpSum, ProcessedDate, Notes, Status) " +
                "VALUES (@id, @name, @m, @l, @d, @notes, 'Pending')",
                new SqlParameter("@id", empId),
                new SqlParameter("@name", name),
                new SqlParameter("@m", monthly),
                new SqlParameter("@l", lump),
                new SqlParameter("@d", DateTime.Today),
                new SqlParameter("@notes", txtNotes.Text.Trim()));

            if (n > 0)
            {
                AuditLogger.Log("Payout Pending", empId, name,
                    string.Format("Payout manually processed for '{0}'. Monthly: BDT {1:N2}, Lump Sum: BDT {2:N2}. Status: Pending.",
                        name, monthly, lump));
                MessageBox.Show(
                    string.Format("Payout created and set to Pending.\n\nMonthly Pension : BDT {0:N2}\nLump Sum        : BDT {1:N2}\n\nPlease use 'Approve Selected' to finalize.", monthly, lump),
                    "Pending Payout Created");
                pnlForm.Visible = false;
                LoadGrid();
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Please select a payout row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = ((DataTable)dgv.DataSource).Rows[dgv.CurrentRow.Index];
            string status = row["Status"].ToString();
            int id = Convert.ToInt32(row["Id"]);
            string name = row["FullName"].ToString();
            string empId = row["EmployeeId"].ToString();

            if (status != "Pending")
            {
                MessageBox.Show("Only Pending payouts can be approved.", "Not Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                string.Format("Approve payout for '{0}'?\n\nMonthly : BDT {1:N2}\nLump Sum: BDT {2:N2}",
                    name, Convert.ToDecimal(row["MonthlyPension"]), Convert.ToDecimal(row["LumpSum"])),
                "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            _db.Execute("UPDATE Payouts SET Status='Approved' WHERE Id=@id", new SqlParameter("@id", id));

            AuditLogger.Log("Payout Approved", empId, name,
                string.Format("Payout approved for '{0}' (ID: {1}). Monthly: BDT {2:N2}, Lump Sum: BDT {3:N2}.",
                    name, empId,
                    Convert.ToDecimal(row["MonthlyPension"]),
                    Convert.ToDecimal(row["LumpSum"])));

            MessageBox.Show(string.Format("Payout for '{0}' has been approved.", name), "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadGrid();
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null)
            {
                MessageBox.Show("Please select a payout row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = ((DataTable)dgv.DataSource).Rows[dgv.CurrentRow.Index];
            string status = row["Status"].ToString();
            int id = Convert.ToInt32(row["Id"]);
            string name = row["FullName"].ToString();
            string empId = row["EmployeeId"].ToString();

            if (status != "Pending")
            {
                MessageBox.Show("Only Pending payouts can be rejected.", "Not Pending", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                string.Format("Reject payout for '{0}'?\nThis will mark it as Rejected.", name),
                "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            _db.Execute("UPDATE Payouts SET Status='Rejected' WHERE Id=@id", new SqlParameter("@id", id));

            AuditLogger.Log("Payout Rejected", empId, name,
                string.Format("Payout rejected for '{0}' (ID: {1}).", name, empId));

            MessageBox.Show(string.Format("Payout for '{0}' has been rejected.", name), "Rejected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadGrid();
        }
    }
}