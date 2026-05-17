using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;
using PensionMgmt.Services;
using PensionMgmt.Session;

namespace PensionMgmt.Forms
{
    public class UserControlPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();

        private TabControl tabControl;
        private TabPage tabPending, tabUsers;
        private DataGridView dgvPending;
        private Label lblPendingCount;
        private DataGridView dgvUsers;
        private Panel pnlAddForm;
        private TextBox txtEmpId, txtName, txtPassword;
        private ComboBox cmbRole;

        public UserControlPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadPendingGrid();
            LoadUsersGrid();
        }

        private void BuildUI()
        {
            Label lHead = new Label
            {
                Text = "User Control",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(20, 14, 400, 34)
            };
            this.Controls.Add(lHead);

            tabControl = new TabControl
            {
                Bounds = new Rectangle(20, 56, this.Width - 40, this.Height - 76),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5f)
            };

            tabPending = new TabPage("Pending Registrations");
            tabUsers = new TabPage("Active Users");

            BuildPendingTab();
            BuildUsersTab();

            tabControl.TabPages.Add(tabPending);
            tabControl.TabPages.Add(tabUsers);
            this.Controls.Add(tabControl);
        }

        private void BuildPendingTab()
        {
            lblPendingCount = new Label
            {
                Text = "",
                ForeColor = Color.OrangeRed,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Bounds = new Rectangle(10, 10, 600, 22)
            };
            tabPending.Controls.Add(lblPendingCount);

            dgvPending = new DataGridView
            {
                Bounds = new Rectangle(10, 38, tabPending.Width - 20, 280),
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
            tabPending.Controls.Add(dgvPending);

            Button btnApprove = new Button { Text = "Approve Selected", Bounds = new Rectangle(10, 328, 160, 32), BackColor = Color.FromArgb(0, 130, 100), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnApprove.FlatAppearance.BorderSize = 0;
            btnApprove.Click += BtnApprove_Click;
            tabPending.Controls.Add(btnApprove);

            Button btnReject = new Button { Text = "Reject Selected", Bounds = new Rectangle(180, 328, 160, 32), BackColor = Color.Firebrick, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnReject.FlatAppearance.BorderSize = 0;
            btnReject.Click += BtnReject_Click;
            tabPending.Controls.Add(btnReject);

            Button btnRefresh = new Button { Text = "Refresh", Bounds = new Rectangle(350, 328, 100, 32), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnRefresh.Click += (s, e) => LoadPendingGrid();
            tabPending.Controls.Add(btnRefresh);

            Label lNote = new Label { Text = "Note: System Admin registration requests can only be approved by an existing System Admin.", ForeColor = Color.DimGray, AutoSize = false, Bounds = new Rectangle(10, 370, 600, 20) };
            tabPending.Controls.Add(lNote);
        }

        private void BuildUsersTab()
        {
            bool canEdit = (CurrentUser.Role == UserRole.SystemAdmin);

            if (canEdit)
            {
                Button btnAdd = new Button { Text = "+ Add User Manually", Bounds = new Rectangle(10, 10, 180, 30), BackColor = Color.FromArgb(25, 45, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btnAdd.FlatAppearance.BorderSize = 0;
                btnAdd.Click += (s, e) => { txtEmpId.Clear(); txtName.Clear(); txtPassword.Clear(); cmbRole.SelectedIndex = 0; pnlAddForm.Visible = true; };
                tabUsers.Controls.Add(btnAdd);
            }

            dgvUsers = new DataGridView
            {
                Bounds = new Rectangle(10, canEdit ? 50 : 10, tabUsers.Width - 20, 280),
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
            tabUsers.Controls.Add(dgvUsers);

            if (canEdit)
            {
                Button btnDel = new Button { Text = "Delete Selected User", Bounds = new Rectangle(10, 340, 200, 30), BackColor = Color.Firebrick, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
                btnDel.FlatAppearance.BorderSize = 0;
                btnDel.Click += BtnDeleteUser_Click;
                tabUsers.Controls.Add(btnDel);

                pnlAddForm = new Panel { BackColor = Color.White, Bounds = new Rectangle(10, 380, 520, 230), Visible = false };
                BuildAddFormPanel();
                tabUsers.Controls.Add(pnlAddForm);
            }
        }

        private void BuildAddFormPanel()
        {
            Label lTitle = new Label { Text = "Add New User Manually", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.FromArgb(25, 45, 80), Bounds = new Rectangle(10, 8, 380, 28) };
            int lw = 160, tw = 200, col = 10, y = 44;

            Label l1 = new Label { Text = "Employee ID (10 digits) *", Bounds = new Rectangle(col, y, lw, 26), TextAlign = ContentAlignment.MiddleLeft };
            txtEmpId = new TextBox { Bounds = new Rectangle(col + lw + 4, y, tw, 26), MaxLength = 10 }; y += 36;

            Label l2 = new Label { Text = "Full Name *", Bounds = new Rectangle(col, y, lw, 26), TextAlign = ContentAlignment.MiddleLeft };
            txtName = new TextBox { Bounds = new Rectangle(col + lw + 4, y, tw, 26) }; y += 36;

            Label l3 = new Label { Text = "Role *", Bounds = new Rectangle(col, y, lw, 26), TextAlign = ContentAlignment.MiddleLeft };
            cmbRole = new ComboBox { Bounds = new Rectangle(col + lw + 4, y, tw, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRole.Items.AddRange(new[] { "System Admin", "Pension Admin", "Pension Manager", "Pension Holder" });
            cmbRole.SelectedIndex = 1; y += 36;

            Label l4 = new Label { Text = "Password (6 digits) *", Bounds = new Rectangle(col, y, lw, 26), TextAlign = ContentAlignment.MiddleLeft };
            txtPassword = new TextBox { Bounds = new Rectangle(col + lw + 4, y, tw, 26), UseSystemPasswordChar = true, MaxLength = 6 }; y += 40;

            Button btnSave = new Button { Text = "Save User", Bounds = new Rectangle(col, y, 110, 30), BackColor = Color.FromArgb(25, 45, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSaveUser_Click;

            Button btnCancel = new Button { Text = "Cancel", Bounds = new Rectangle(col + 120, y, 80, 30), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCancel.Click += (s, e) => pnlAddForm.Visible = false;

            pnlAddForm.Controls.AddRange(new Control[] { lTitle, l1, txtEmpId, l2, txtName, l3, cmbRole, l4, txtPassword, btnSave, btnCancel });
        }

        private void LoadPendingGrid()
        {
            DataTable dt = _db.GetTable(
                "SELECT Id, FullName, EmployeeId, RequestedRole, Department, Rank, BasicSalary, DateOfBirth, JoiningDate, RequestedOn " +
                "FROM PendingRegistrations WHERE Status = 'Pending' ORDER BY RequestedOn");
            dgvPending.DataSource = dt;

            int count = dt.Rows.Count;
            lblPendingCount.Text = count == 0
                ? "No pending registration requests."
                : string.Format("{0} pending request(s) awaiting approval.", count);
            tabPending.Text = count > 0
                ? string.Format("Pending Registrations ({0})", count)
                : "Pending Registrations";
        }

        private void LoadUsersGrid()
        {
            dgvUsers.DataSource = _db.GetTable(
                "SELECT FullName, EmployeeId, Role FROM Users ORDER BY Role, FullName");
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvPending.CurrentRow == null)
            {
                MessageBox.Show("Please select a registration request first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = ((DataTable)dgvPending.DataSource).Rows[dgvPending.CurrentRow.Index];
            int pendingId = Convert.ToInt32(row["Id"]);
            string name = row["FullName"].ToString();
            string role = row["RequestedRole"].ToString();

            string dept = row["Department"] != DBNull.Value ? row["Department"].ToString() : "";
            string rank = row["Rank"] != DBNull.Value ? row["Rank"].ToString() : "Assistant";
            decimal salary = row["BasicSalary"] != DBNull.Value ? Convert.ToDecimal(row["BasicSalary"]) : 0;
            DateTime dob = row["DateOfBirth"] != DBNull.Value ? Convert.ToDateTime(row["DateOfBirth"]) : DateTime.Today.AddYears(-30);
            DateTime joining = row["JoiningDate"] != DBNull.Value ? Convert.ToDateTime(row["JoiningDate"]) : DateTime.Today;

            // Permission check for System Admin approval
            if (role == "System Admin" && CurrentUser.Role != UserRole.SystemAdmin)
            {
                MessageBox.Show(
                    "Only an existing System Admin can approve a System Admin registration request.\nYou do not have permission to approve this request.",
                    "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get password from DB
            DataTable dtPass = _db.GetTable("SELECT Password FROM PendingRegistrations WHERE Id = @id", new SqlParameter("@id", pendingId));
            if (dtPass.Rows.Count == 0) return;
            string pass = dtPass.Rows[0]["Password"].ToString();

            // Confirm dialog
            DialogResult confirm = MessageBox.Show(
                string.Format(
                    "Approve registration for:\n\nName: {0}\nRole: {1}\nDepartment: {2}\nRank: {3}\nBasic Salary: BDT {4:N0}\n\nA new Employee ID will be assigned automatically by the system.",
                    name, role, dept, rank, salary),
                "Confirm Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            string realEmpId = null;

            // STEP 1 — For roles that have employee records, insert into Employees first
            //          Do NOT include EmployeeId — it is a computed column, SQL generates it automatically
            if (role == "Pension Holder" || role == "Pension Manager")
            {
                _db.Execute(
                    "INSERT INTO Employees (FullName, Department, Rank, BasicSalary, DateOfBirth, JoiningDate, Status) " +
                    "VALUES (@name, @dept, @rank, @sal, @dob, @join, 'Active')",
                    new SqlParameter("@name", name),
                    new SqlParameter("@dept", string.IsNullOrEmpty(dept) ? (object)DBNull.Value : dept),
                    new SqlParameter("@rank", rank),
                    new SqlParameter("@sal", salary),
                    new SqlParameter("@dob", dob),
                    new SqlParameter("@join", joining));

                // STEP 2 — Read back the auto-generated EmployeeId SQL just created
                object generatedId = _db.ExecuteScalar(
                    "SELECT TOP 1 EmployeeId FROM Employees WHERE FullName = @name AND JoiningDate = @join ORDER BY Id DESC",
                    new SqlParameter("@name", name),
                    new SqlParameter("@join", joining));

                if (generatedId == null || generatedId == DBNull.Value)
                {
                    MessageBox.Show("Failed to retrieve the generated Employee ID. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                realEmpId = generatedId.ToString();
            }
            else
            {
                // System Admin / Pension Admin don't have employee records
                // Generate a unique ID for them based on their role prefix
                string prefix = (role == "System Admin") ? "1" : "2";
                object maxId = _db.ExecuteScalar(
                    string.Format("SELECT MAX(CAST(EmployeeId AS BIGINT)) FROM Users WHERE EmployeeId LIKE '{0}%'", prefix));
                long nextId = (maxId == null || maxId == DBNull.Value)
                    ? long.Parse(prefix + "000000001")
                    : Convert.ToInt64(maxId) + 1;
                realEmpId = nextId.ToString();
            }

            // STEP 3 — Check if this EmployeeId already exists in Users (safety check)
            object already = _db.ExecuteScalar("SELECT COUNT(*) FROM Users WHERE EmployeeId = @id", new SqlParameter("@id", realEmpId));
            if (Convert.ToInt32(already) > 0)
            {
                MessageBox.Show("Generated Employee ID already exists. Please try again.", "Conflict", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // STEP 4 — Insert into Users with the real EmployeeId
            _db.Execute("INSERT INTO Users (FullName, EmployeeId, Role, Password) VALUES (@n, @id, @r, @p)",
                new SqlParameter("@n", name),
                new SqlParameter("@id", realEmpId),
                new SqlParameter("@r", role),
                new SqlParameter("@p", pass));

            // STEP 5 — Insert into role-specific auth table
            string authTable = RoleToTable(role);
            if (authTable == "PensionHolder")
                _db.Execute("INSERT INTO PensionHolder (UserName, Password, EmployeeId) VALUES (@u, @p, @eid)",
                    new SqlParameter("@u", realEmpId),
                    new SqlParameter("@p", pass),
                    new SqlParameter("@eid", realEmpId));
            else
                _db.Execute(string.Format("INSERT INTO [{0}] (UserName, Password) VALUES (@u, @p)", authTable),
                    new SqlParameter("@u", realEmpId),
                    new SqlParameter("@p", pass));

            // STEP 6 — Mark pending registration as approved
            _db.Execute("UPDATE PendingRegistrations SET Status='Approved', ReviewedBy=@by, ReviewedAt=@at WHERE Id=@id",
                new SqlParameter("@by", CurrentUser.Username),
                new SqlParameter("@at", DateTime.Now),
                new SqlParameter("@id", pendingId));

            AuditLogger.Log("User Approved", realEmpId, name,
                string.Format("Registration approved for '{0}'. Assigned Employee ID: {1}. Role: '{2}'.", name, realEmpId, role));

            MessageBox.Show(
                string.Format(
                    "User '{0}' approved successfully!\n\n" +
                    "Assigned Employee ID: {1}\n\n" +
                    "Please inform them of their new Employee ID.\n" +
                    "They can log in using this ID and their chosen password.", name, realEmpId),
                "Approved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadPendingGrid();
            LoadUsersGrid();
        }

        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvPending.CurrentRow == null)
            {
                MessageBox.Show("Please select a registration request first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = ((DataTable)dgvPending.DataSource).Rows[dgvPending.CurrentRow.Index];
            int id = Convert.ToInt32(row["Id"]);
            string name = row["FullName"].ToString();
            string empId = row["EmployeeId"].ToString();

            DialogResult confirm = MessageBox.Show(
                string.Format("Reject registration request from '{0}'?", name),
                "Confirm Rejection", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            _db.Execute("UPDATE PendingRegistrations SET Status='Rejected', ReviewedBy=@by, ReviewedAt=@at WHERE Id=@id",
                new SqlParameter("@by", CurrentUser.Username),
                new SqlParameter("@at", DateTime.Now),
                new SqlParameter("@id", id));

            AuditLogger.Log("User Rejected", empId, name,
                string.Format("Registration request rejected for '{0}' (Temp ID: {1}).", name, empId));

            MessageBox.Show("Registration request rejected.", "Rejected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPendingGrid();
        }

        private void BtnSaveUser_Click(object sender, EventArgs e)
        {
            string id = txtEmpId.Text.Trim();
            string name = txtName.Text.Trim();
            string role = cmbRole.Text;
            string pass = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            { MessageBox.Show("Please fill all required fields.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (id.Length != 10 || !long.TryParse(id, out _))
            { MessageBox.Show("Employee ID must be exactly 10 numeric digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (pass.Length != 6 || !int.TryParse(pass, out _))
            { MessageBox.Show("Password must be exactly 6 numeric digits.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            object exists = _db.ExecuteScalar("SELECT COUNT(*) FROM Users WHERE EmployeeId = @id", new SqlParameter("@id", id));
            if (Convert.ToInt32(exists) > 0)
            { MessageBox.Show("A user with this Employee ID already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            _db.Execute("INSERT INTO Users (FullName, EmployeeId, Role, Password) VALUES (@n, @id, @r, @p)",
                new SqlParameter("@n", name), new SqlParameter("@id", id),
                new SqlParameter("@r", role), new SqlParameter("@p", pass));

            string authTable = RoleToTable(role);
            if (authTable == "PensionHolder")
                _db.Execute("INSERT INTO PensionHolder (UserName, Password, EmployeeId) VALUES (@u, @p, @eid)",
                    new SqlParameter("@u", id), new SqlParameter("@p", pass), new SqlParameter("@eid", id));
            else
                _db.Execute(string.Format("INSERT INTO [{0}] (UserName, Password) VALUES (@u, @p)", authTable),
                    new SqlParameter("@u", id), new SqlParameter("@p", pass));

            AuditLogger.Log("User Added Manually", id, name,
                string.Format("User '{0}' (ID: {1}) manually added with role '{2}'.", name, id, role));

            MessageBox.Show("User added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            pnlAddForm.Visible = false;
            LoadUsersGrid();
        }

        private void BtnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null)
            { MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            DataRow row = ((DataTable)dgvUsers.DataSource).Rows[dgvUsers.CurrentRow.Index];
            string empId = row["EmployeeId"].ToString();
            string role = row["Role"].ToString();
            string name = row["FullName"].ToString();

            if (role == "System Admin")
            {
                object adminCount = _db.ExecuteScalar("SELECT COUNT(*) FROM SystemAdmin");
                if (Convert.ToInt32(adminCount) <= 1)
                {
                    MessageBox.Show("Cannot delete the last System Admin account.\nThere must always be at least one System Admin in the system.",
                        "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (MessageBox.Show(string.Format("Delete user '{0}' ({1})?\nThis cannot be undone.", name, empId),
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            string authTable = RoleToTable(role);
            _db.Execute(string.Format("DELETE FROM [{0}] WHERE UserName=@id", authTable), new SqlParameter("@id", empId));
            _db.Execute("DELETE FROM Users WHERE EmployeeId=@id", new SqlParameter("@id", empId));

            AuditLogger.Log("User Deleted", empId, name,
                string.Format("User '{0}' (ID: {1}, Role: {2}) was deleted from the system.", name, empId, role));

            MessageBox.Show("User deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadUsersGrid();
        }

        private string RoleToTable(string role)
        {
            switch (role)
            {
                case "System Admin": return "SystemAdmin";
                case "Pension Admin": return "PensionAdmin";
                case "Pension Manager": return "Manager";
                case "Pension Holder": return "PensionHolder";
                default: return "PensionHolder";
            }
        }
    }
}