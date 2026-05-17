using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;
using PensionMgmt.Session;

namespace PensionMgmt.Forms
{
    public class ManageAccountPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();

        // Account section
        private TextBox txtEmpId, txtName, txtPass, txtConfirm;

        // Employee record section
        private TextBox txtDept, txtRank, txtSalary, txtStatus;
        private DateTimePicker dtpDob, dtpJoining;
        private Label lblEmpMsg;

        // Status feedback
        private Label lblStatus;

        public ManageAccountPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadData();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  BUILD UI
        // ─────────────────────────────────────────────────────────────────────

        private void BuildUI()
        {
            this.Controls.Add(new Label
            {
                Text = "Manage Account",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(20, 14, 500, 34)
            });
            this.Controls.Add(new Label
            {
                Text = "Update your credentials and employee details below. Employee ID and Role are read-only.",
                ForeColor = Color.DimGray,
                Font = new Font("Segoe UI", 9f),
                AutoSize = false,
                Bounds = new Rectangle(20, 50, 700, 20)
            });

            // ── SECTION 1: Account Information ────────────────────────────────
            Panel pnlAccount = MakeCard(20, 80, 860, 270);
            this.Controls.Add(pnlAccount);
            SectionHeader(pnlAccount, "Account Information", 14, 10);

            int y = 42;

            AddLabel(pnlAccount, "Employee ID", 14, y);
            AddLabel(pnlAccount, "Role", 480, y);
            y += 20;

            txtEmpId = new TextBox
            {
                Bounds = new Rectangle(14, y, 340, 26),
                ReadOnly = true,
                BackColor = Color.FromArgb(230, 230, 230),
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlAccount.Controls.Add(txtEmpId);

            string roleText = CurrentUser.Role.ToString()
                .Replace("SystemAdmin", "System Admin")
                .Replace("PensionAdmin", "Pension Admin")
                .Replace("PensionManager", "Pension Manager")
                .Replace("PensionHolder", "Pension Holder");
            pnlAccount.Controls.Add(new Label
            {
                Text = roleText,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(480, y, 360, 26)
            });
            y += 36;

            AddLabel(pnlAccount, "Full Name", 14, y);
            y += 20;
            txtName = new TextBox
            {
                Bounds = new Rectangle(14, y, 806, 26),
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlAccount.Controls.Add(txtName);
            y += 36;

            AddLabel(pnlAccount, "New Password (6 digits)  —  leave blank to keep current", 14, y);
            AddLabel(pnlAccount, "Confirm Password", 430, y);
            y += 20;
            txtPass = new TextBox
            {
                Bounds = new Rectangle(14, y, 400, 26),
                UseSystemPasswordChar = true,
                MaxLength = 6,
                Font = new Font("Segoe UI", 9.5f)
            };
            txtConfirm = new TextBox
            {
                Bounds = new Rectangle(430, y, 390, 26),
                UseSystemPasswordChar = true,
                MaxLength = 6,
                Font = new Font("Segoe UI", 9.5f)
            };
            pnlAccount.Controls.Add(txtPass);
            pnlAccount.Controls.Add(txtConfirm);

            // ── SECTION 2: Employee Record (fully editable) ───────────────────
            Panel pnlEmployee = MakeCard(20, 368, 860, 260);
            this.Controls.Add(pnlEmployee);
            SectionHeader(pnlEmployee, "Employee Record", 14, 10);

            lblEmpMsg = new Label
            {
                Text = "",
                ForeColor = Color.OrangeRed,
                Font = new Font("Segoe UI", 9f),
                AutoSize = false,
                Bounds = new Rectangle(14, 38, 820, 20),
                Visible = false
            };
            pnlEmployee.Controls.Add(lblEmpMsg);

            int ey = 62;

            AddLabel(pnlEmployee, "Department", 14, ey);
            AddLabel(pnlEmployee, "Rank", 450, ey);
            ey += 20;
            txtDept = new TextBox { Bounds = new Rectangle(14, ey, 410, 26), Font = new Font("Segoe UI", 9.5f) };
            txtRank = new TextBox { Bounds = new Rectangle(450, ey, 390, 26), Font = new Font("Segoe UI", 9.5f) };
            pnlEmployee.Controls.Add(txtDept);
            pnlEmployee.Controls.Add(txtRank);
            ey += 36;

            AddLabel(pnlEmployee, "Basic Salary (BDT)", 14, ey);
            AddLabel(pnlEmployee, "Status", 450, ey);
            ey += 20;
            txtSalary = new TextBox { Bounds = new Rectangle(14, ey, 410, 26), Font = new Font("Segoe UI", 9.5f) };
            txtStatus = new TextBox { Bounds = new Rectangle(450, ey, 390, 26), Font = new Font("Segoe UI", 9.5f) };
            pnlEmployee.Controls.Add(txtSalary);
            pnlEmployee.Controls.Add(txtStatus);
            ey += 36;

            AddLabel(pnlEmployee, "Date of Birth", 14, ey);
            AddLabel(pnlEmployee, "Joining Date", 450, ey);
            ey += 20;
            dtpDob = new DateTimePicker
            {
                Bounds = new Rectangle(14, ey, 410, 26),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9.5f),
                Value = DateTime.Today.AddYears(-30)
            };
            dtpJoining = new DateTimePicker
            {
                Bounds = new Rectangle(450, ey, 390, 26),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 9.5f),
                Value = DateTime.Today
            };
            pnlEmployee.Controls.Add(dtpDob);
            pnlEmployee.Controls.Add(dtpJoining);

            // ── Buttons ────────────────────────────────────────────────────────
            int by = 646;
            Button btnRefresh = MakeBtn("Refresh", 20, by, 120, Color.FromArgb(100, 116, 139));
            Button btnSave = MakeBtn("Save All Changes", 150, by, 180, Color.FromArgb(25, 45, 80));
            Button btnDelete = MakeBtn("Delete Account", 660, by, 160, Color.Firebrick);

            btnRefresh.Click += (s, e) => { LoadData(); SetStatus("Refreshed.", false); };
            btnSave.Click += BtnSave_Click;
            btnDelete.Click += BtnDelete_Click;

            this.Controls.Add(btnRefresh);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnDelete);

            lblStatus = new Label
            {
                Text = "",
                Bounds = new Rectangle(20, by + 44, 600, 22),
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                AutoSize = false
            };
            this.Controls.Add(lblStatus);
        }

        // ─────────────────────────────────────────────────────────────────────
        //  LOAD DATA
        // ─────────────────────────────────────────────────────────────────────

        private void LoadData()
        {
            txtPass.Text = "";
            txtConfirm.Text = "";
            SetStatus("", false);
            LoadAccountInfo();
            LoadEmployeeRecord();
        }

        private void LoadAccountInfo()
        {
            try
            {
                DataTable dt = _db.GetTable(
                    "SELECT FullName, EmployeeId FROM Users WHERE EmployeeId = @id",
                    new SqlParameter("@id", CurrentUser.Username));

                if (dt.Rows.Count > 0)
                {
                    txtEmpId.Text = dt.Rows[0]["EmployeeId"].ToString();
                    txtName.Text = dt.Rows[0]["FullName"].ToString();
                }
                else
                {
                    txtEmpId.Text = CurrentUser.Username;
                    txtName.Text = CurrentUser.FullName ?? "";
                }
            }
            catch (Exception ex)
            {
                SetStatus("Error loading account: " + ex.Message, true);
            }
        }

        private void LoadEmployeeRecord()
        {
            try
            {
                string empId = GetEmpId();

                DataTable dt = _db.GetTable(
                    "SELECT Department, Rank, BasicSalary, DateOfBirth, JoiningDate, Status " +
                    "FROM Employees WHERE EmployeeId = @id",
                    new SqlParameter("@id", empId));

                if (dt.Rows.Count == 0)
                {
                    lblEmpMsg.Text = "No employee record found. Fill in the details below and click Save All Changes to create one.";
                    lblEmpMsg.Visible = true;
                    txtDept.Text = txtRank.Text = txtSalary.Text = txtStatus.Text = "";
                    dtpDob.Value = DateTime.Today.AddYears(-30);
                    dtpJoining.Value = DateTime.Today;
                    return;
                }

                lblEmpMsg.Visible = false;
                DataRow row = dt.Rows[0];
                txtDept.Text = row["Department"].ToString();
                txtRank.Text = row["Rank"].ToString();
                txtSalary.Text = Convert.ToDecimal(row["BasicSalary"]).ToString("0.##");
                txtStatus.Text = row["Status"].ToString();
                dtpDob.Value = Convert.ToDateTime(row["DateOfBirth"]);
                dtpJoining.Value = Convert.ToDateTime(row["JoiningDate"]);
            }
            catch (Exception ex)
            {
                lblEmpMsg.Text = "Error loading employee record: " + ex.Message;
                lblEmpMsg.Visible = true;
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  SAVE
        // ─────────────────────────────────────────────────────────────────────

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string newName = txtName.Text.Trim();
            string newPass = txtPass.Text.Trim();
            string newConfirm = txtConfirm.Text.Trim();

            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Full Name cannot be empty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus(); return;
            }

            bool changingPassword = !string.IsNullOrEmpty(newPass) || !string.IsNullOrEmpty(newConfirm);
            if (changingPassword)
            {
                if (newPass.Length != 6 || !int.TryParse(newPass, out _))
                {
                    MessageBox.Show("Password must be exactly 6 numeric digits.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPass.Focus(); return;
                }
                if (newPass != newConfirm)
                {
                    MessageBox.Show("Passwords do not match.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtConfirm.Focus(); return;
                }
            }

            string dept = txtDept.Text.Trim();
            string rank = txtRank.Text.Trim();
            string status = txtStatus.Text.Trim();
            string salaryStr = txtSalary.Text.Trim();
            bool hasEmpData = !string.IsNullOrEmpty(dept) || !string.IsNullOrEmpty(rank) || !string.IsNullOrEmpty(salaryStr);
            decimal salary = 0;

            if (hasEmpData)
            {
                if (string.IsNullOrEmpty(dept))
                {
                    MessageBox.Show("Department cannot be empty if you are filling employee details.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDept.Focus(); return;
                }
                if (string.IsNullOrEmpty(rank))
                {
                    MessageBox.Show("Rank cannot be empty if you are filling employee details.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtRank.Focus(); return;
                }
                if (!decimal.TryParse(salaryStr, out salary) || salary <= 0)
                {
                    MessageBox.Show("Basic Salary must be a valid positive number.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtSalary.Focus(); return;
                }
                if (dtpDob.Value.Date >= dtpJoining.Value.Date)
                {
                    MessageBox.Show("Joining Date must be after Date of Birth.", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                // Save account info
                if (changingPassword)
                {
                    _db.Execute(
                        "UPDATE Users SET FullName=@name, Password=@pass WHERE EmployeeId=@id",
                        new SqlParameter("@name", newName),
                        new SqlParameter("@pass", newPass),
                        new SqlParameter("@id", CurrentUser.Username));

                    _db.Execute(
                        string.Format("UPDATE [{0}] SET Password=@pass WHERE UserName=@id", GetRoleTable()),
                        new SqlParameter("@pass", newPass),
                        new SqlParameter("@id", CurrentUser.Username));
                }
                else
                {
                    _db.Execute(
                        "UPDATE Users SET FullName=@name WHERE EmployeeId=@id",
                        new SqlParameter("@name", newName),
                        new SqlParameter("@id", CurrentUser.Username));
                }

                CurrentUser.FullName = newName;
                txtPass.Text = txtConfirm.Text = "";

                // Save employee record
                if (hasEmpData)
                {
                    string empId = GetEmpId();
                    string finalStatus = string.IsNullOrEmpty(status) ? "Active" : status;

                    object exists = _db.ExecuteScalar(
                        "SELECT COUNT(*) FROM Employees WHERE EmployeeId=@id",
                        new SqlParameter("@id", empId));

                    if (Convert.ToInt32(exists) > 0)
                    {
                        _db.Execute(
                            "UPDATE Employees SET FullName=@fn, Department=@dept, Rank=@rank, " +
                            "BasicSalary=@sal, DateOfBirth=@dob, JoiningDate=@join, Status=@st " +
                            "WHERE EmployeeId=@id",
                            new SqlParameter("@fn", newName),
                            new SqlParameter("@dept", dept),
                            new SqlParameter("@rank", rank),
                            new SqlParameter("@sal", salary),
                            new SqlParameter("@dob", dtpDob.Value.Date),
                            new SqlParameter("@join", dtpJoining.Value.Date),
                            new SqlParameter("@st", finalStatus),
                            new SqlParameter("@id", empId));
                    }
                    else
                    {
                        _db.Execute(
                            "INSERT INTO Employees (FullName, EmployeeId, Department, Rank, BasicSalary, DateOfBirth, JoiningDate, Status) " +
                            "VALUES (@fn, @id, @dept, @rank, @sal, @dob, @join, @st)",
                            new SqlParameter("@fn", newName),
                            new SqlParameter("@id", empId),
                            new SqlParameter("@dept", dept),
                            new SqlParameter("@rank", rank),
                            new SqlParameter("@sal", salary),
                            new SqlParameter("@dob", dtpDob.Value.Date),
                            new SqlParameter("@join", dtpJoining.Value.Date),
                            new SqlParameter("@st", finalStatus));
                    }

                    lblEmpMsg.Visible = false;
                }

                SetStatus("All changes saved successfully.", false);
            }
            catch (Exception ex)
            {
                SetStatus("Save failed: " + ex.Message, true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  DELETE ACCOUNT
        // ─────────────────────────────────────────────────────────────────────

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == UserRole.SystemAdmin)
            {
                object count = _db.ExecuteScalar("SELECT COUNT(*) FROM SystemAdmin");
                if (Convert.ToInt32(count) <= 1)
                {
                    MessageBox.Show(
                        "You are the last System Admin and cannot delete your own account.\n" +
                        "Create another System Admin account first.",
                        "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (MessageBox.Show(
                    "Are you sure you want to permanently delete your account?\n\nYou will be logged out immediately.",
                    "Delete Account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                _db.Execute(
                    string.Format("DELETE FROM [{0}] WHERE UserName=@id", GetRoleTable()),
                    new SqlParameter("@id", CurrentUser.Username));

                _db.Execute("DELETE FROM Users WHERE EmployeeId=@id",
                    new SqlParameter("@id", CurrentUser.Username));

                MessageBox.Show("Account deleted. Returning to login.",
                    "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CurrentUser.Clear();
                new LoginForm().Show();
                this.FindForm()?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  HELPERS
        // ─────────────────────────────────────────────────────────────────────

        private string GetEmpId()
        {
            if (CurrentUser.Role == UserRole.PensionHolder)
            {
                object linked = _db.ExecuteScalar(
                    "SELECT EmployeeId FROM PensionHolder WHERE UserName=@u",
                    new SqlParameter("@u", CurrentUser.Username));

                if (linked != null && linked != DBNull.Value && !string.IsNullOrEmpty(linked.ToString()))
                    return linked.ToString();
            }
            return CurrentUser.Username;
        }

        private string GetRoleTable()
        {
            switch (CurrentUser.Role)
            {
                case UserRole.SystemAdmin: return "SystemAdmin";
                case UserRole.PensionAdmin: return "PensionAdmin";
                case UserRole.PensionManager: return "Manager";
                case UserRole.PensionHolder: return "PensionHolder";
                default: return "Users";
            }
        }

        private void SetStatus(string msg, bool isError)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = isError ? Color.Firebrick : Color.DarkGreen;
        }

        private Panel MakeCard(int x, int y, int w, int h)
            => new Panel { BackColor = Color.White, Bounds = new Rectangle(x, y, w, h) };

        private Button MakeBtn(string text, int x, int y, int w, Color bg)
        {
            var b = new Button
            {
                Text = text,
                Bounds = new Rectangle(x, y, w, 36),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void SectionHeader(Panel parent, string title, int x, int y)
        {
            parent.Controls.Add(new Panel
            {
                BackColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(x, y, 4, 22)
            });
            parent.Controls.Add(new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(x + 10, y, 400, 22)
            });
        }

        private void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Bounds = new Rectangle(x, y, 430, 20),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold)
            });
        }
    }
}