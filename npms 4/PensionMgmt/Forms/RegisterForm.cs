using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;

namespace PensionMgmt.Forms
{
    public class RegisterForm : Form
    {
        private readonly DbAccess _db = new DbAccess();
        private TextBox txtEmpId, txtName, txtPass, txtConfirm;
        private ComboBox cmbRole;
        private Label lblRoleNote;
        private Label errEmpId, errName, errPass, errConfirm;
        private TextBox txtDept, txtSalary;
        private ComboBox cmbRank;
        private DateTimePicker dtpDob, dtpJoining;
        private Label errDept, errSalary, errDates;

        public RegisterForm()
        {
            BuildUI();
            LoadNextEmployeeId(); // auto-fill the ID on load
        }

        // Fetches the next available EmployeeId from the Employees table
        private void LoadNextEmployeeId()
        {
            try
            {
                object result = _db.ExecuteScalar(
                    "SELECT ISNULL(MAX(CAST(EmployeeId AS BIGINT)), 4000000000) + 1 FROM Employees");
                txtEmpId.Text = result?.ToString() ?? "4000000001";
            }
            catch
            {
                txtEmpId.Text = "4000000001";
            }
        }

        private void BuildUI()
        {
            this.Text = "Govt. Pension Management - Register";
            this.Size = new Size(460, 860);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 50, 90);
            this.Font = new Font("Segoe UI", 9.5f);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new Size(440, 0);

            Label lblTitle = new Label
            {
                Text = "Government Pension Management",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 16, 460, 36)
            };

            Label lblSubtitle = new Label
            {
                Text = "New User Registration",
                ForeColor = Color.LightSkyBlue,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Bounds = new Rectangle(0, 52, 460, 22)
            };

            Panel pnlCard = new Panel
            {
                BackColor = Color.White,
                Location = new Point(50, 82),
                Width = 340
            };

            int y = 16;

            // ── Account Information ──────────────────────────────
            Add(pnlCard, new Label
            {
                Text = "Account Information",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 90),
                AutoSize = false,
                Bounds = new Rectangle(20, y, 300, 18)
            });
            y += 24;

            // Employee ID — visible but NOT editable by user
            Add(pnlCard, FieldLabel("Employee ID (auto-assigned) *", y)); y += 20;
            txtEmpId = new TextBox
            {
                Bounds = new Rectangle(20, y, 300, 26),
                MaxLength = 10,
                Font = new Font("Segoe UI", 9.5f),
                ReadOnly = true,
                BackColor = Color.FromArgb(220, 220, 220),
                ForeColor = Color.FromArgb(60, 60, 60),
                Cursor = Cursors.Default
            };
            Add(pnlCard, txtEmpId); y += 26;
            errEmpId = MakeErr(y);
            Add(pnlCard, errEmpId); y += 20;

            Add(pnlCard, FieldLabel("Full Name *", y)); y += 20;
            txtName = MakeTxt(y);
            Add(pnlCard, txtName); y += 26;
            errName = MakeErr(y);
            Add(pnlCard, errName); y += 20;

            Add(pnlCard, FieldLabel("Requested Role *", y)); y += 20;
            cmbRole = new ComboBox
            {
                Bounds = new Rectangle(20, y, 300, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.AddRange(new[] { "Pension Holder", "Pension Manager", "Pension Admin", "System Admin" });
            cmbRole.SelectedIndex = 0;
            cmbRole.SelectedIndexChanged += CmbRole_SelectedIndexChanged;
            Add(pnlCard, cmbRole); y += 28;

            lblRoleNote = new Label
            {
                Text = "Pension Holders can view their own pension records and calculate their pension.",
                ForeColor = Color.DimGray,
                AutoSize = false,
                Bounds = new Rectangle(20, y, 300, 32)
            };
            Add(pnlCard, lblRoleNote); y += 40;

            Add(pnlCard, FieldLabel("Password (6 digits) *", y)); y += 20;
            txtPass = MakeTxt(y, 6, password: true);
            Add(pnlCard, txtPass); y += 26;
            errPass = MakeErr(y);
            Add(pnlCard, errPass); y += 20;

            Add(pnlCard, FieldLabel("Confirm Password *", y)); y += 20;
            txtConfirm = MakeTxt(y, 6, password: true);
            Add(pnlCard, txtConfirm); y += 26;
            errConfirm = MakeErr(y);
            Add(pnlCard, errConfirm); y += 24;

            Add(pnlCard, Divider(y)); y += 14;

            // ── Employee Details ─────────────────────────────────
            Add(pnlCard, new Label
            {
                Text = "Employee Details  (required for system records)",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 50, 90),
                AutoSize = false,
                Bounds = new Rectangle(20, y, 300, 18)
            });
            y += 24;

            Add(pnlCard, FieldLabel("Department *", y)); y += 20;
            txtDept = MakeTxt(y, 100);
            Add(pnlCard, txtDept); y += 26;
            errDept = MakeErr(y);
            Add(pnlCard, errDept); y += 20;

            Add(pnlCard, FieldLabel("Rank *", y)); y += 20;
            cmbRank = new ComboBox
            {
                Bounds = new Rectangle(20, y, 300, 26),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRank.Items.AddRange(new[] { "Secretary", "Joint Secretary", "Deputy Secretary", "Senior Assistant", "Assistant" });
            cmbRank.SelectedIndex = 4;
            Add(pnlCard, cmbRank); y += 34;

            Add(pnlCard, FieldLabel("Basic Salary (BDT) *", y)); y += 20;
            txtSalary = MakeTxt(y, 20);
            Add(pnlCard, txtSalary); y += 26;
            errSalary = MakeErr(y);
            Add(pnlCard, errSalary); y += 20;

            Add(pnlCard, FieldLabel("Date of Birth *", y)); y += 20;
            dtpDob = new DateTimePicker
            {
                Bounds = new Rectangle(20, y, 300, 26),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddYears(-30)
            };
            Add(pnlCard, dtpDob); y += 34;

            Add(pnlCard, FieldLabel("Joining Date *", y)); y += 20;
            dtpJoining = new DateTimePicker
            {
                Bounds = new Rectangle(20, y, 300, 26),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            Add(pnlCard, dtpJoining); y += 26;
            errDates = MakeErr(y);
            Add(pnlCard, errDates); y += 24;

            Add(pnlCard, Divider(y)); y += 14;

            Button btnRegister = new Button
            {
                Text = "SUBMIT REGISTRATION",
                Bounds = new Rectangle(20, y, 300, 36),
                BackColor = Color.FromArgb(30, 50, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.Click += BtnRegister_Click;
            Add(pnlCard, btnRegister); y += 46;

            Button btnBack = new Button
            {
                Text = "← Back to Login",
                Bounds = new Rectangle(90, y, 160, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(30, 50, 90),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Underline),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => { new LoginForm().Show(); this.Close(); };
            Add(pnlCard, btnBack); y += 36;

            pnlCard.Height = y + 16;
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSubtitle);
            this.Controls.Add(pnlCard);
            this.AcceptButton = btnRegister;
        }

        private void Add(Panel p, Control c) => p.Controls.Add(c);

        private Label FieldLabel(string text, int y) => new Label
        {
            Text = text,
            ForeColor = Color.Black,
            AutoSize = false,
            Bounds = new Rectangle(20, y, 300, 20)
        };

        private TextBox MakeTxt(int y, int maxLen = 200, bool password = false) => new TextBox
        {
            Bounds = new Rectangle(20, y, 300, 26),
            MaxLength = maxLen,
            Font = new Font("Segoe UI", 9.5f),
            PasswordChar = password ? '●' : '\0'
        };

        private Label MakeErr(int y) => new Label
        {
            Text = "",
            ForeColor = Color.Crimson,
            Font = new Font("Segoe UI", 8.5f),
            AutoSize = false,
            Bounds = new Rectangle(20, y, 300, 18)
        };

        private Panel Divider(int y) => new Panel
        {
            BackColor = Color.FromArgb(210, 210, 210),
            Bounds = new Rectangle(20, y, 300, 1)
        };

        private void ClearErrors()
        {
            errEmpId.Text = errName.Text = errPass.Text = errConfirm.Text =
            errDept.Text = errSalary.Text = errDates.Text = "";
        }

        private void CmbRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbRole.Text)
            {
                case "Pension Holder":
                    lblRoleNote.Text = "Pension Holders can view their own pension records and calculate their pension.";
                    lblRoleNote.ForeColor = Color.DimGray; break;
                case "Pension Manager":
                    lblRoleNote.Text = "Pension Managers can manage employees, payouts, and retirement records.";
                    lblRoleNote.ForeColor = Color.DimGray; break;
                case "Pension Admin":
                    lblRoleNote.Text = "Pension Admins can also approve new user registrations (except System Admin requests).";
                    lblRoleNote.ForeColor = Color.DimGray; break;
                case "System Admin":
                    lblRoleNote.Text = "IMPORTANT: System Admin requests can only be approved by an existing System Admin.";
                    lblRoleNote.ForeColor = Color.DarkRed; break;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            ClearErrors();

            string empId = txtEmpId.Text.Trim(); // read-only, set by system
            string name = txtName.Text.Trim();
            string role = cmbRole.Text;
            string pass = txtPass.Text.Trim();
            string confirm = txtConfirm.Text.Trim();
            string dept = txtDept.Text.Trim();
            string rank = cmbRank.Text;
            string salary = txtSalary.Text.Trim();

            // Validate
            if (string.IsNullOrEmpty(name))
            { errName.Text = "Full Name is required."; txtName.Focus(); return; }
            if (string.IsNullOrEmpty(pass))
            { errPass.Text = "Password is required."; txtPass.Focus(); return; }
            if (pass.Length != 6 || !int.TryParse(pass, out _))
            { errPass.Text = "Must be exactly 6 numeric digits."; txtPass.Focus(); return; }
            if (string.IsNullOrEmpty(confirm))
            { errConfirm.Text = "Please confirm your password."; txtConfirm.Focus(); return; }
            if (pass != confirm)
            { errConfirm.Text = "Passwords do not match."; txtConfirm.Focus(); return; }
            if (string.IsNullOrEmpty(dept))
            { errDept.Text = "Department is required."; txtDept.Focus(); return; }
            if (!decimal.TryParse(salary, out decimal salaryVal) || salaryVal <= 0)
            { errSalary.Text = "Enter a valid positive salary."; txtSalary.Focus(); return; }
            if (dtpDob.Value.Date >= dtpJoining.Value.Date)
            { errDates.Text = "Joining Date must be after Date of Birth."; return; }

            // Safety check — ID shown should not already be taken
            object existsInUsers = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM Users WHERE EmployeeId = @id",
                new SqlParameter("@id", empId));
            if (Convert.ToInt32(existsInUsers) > 0)
            {
                // Refresh the ID and warn
                LoadNextEmployeeId();
                errEmpId.Text = "ID conflict detected. A new ID has been assigned, please try again.";
                return;
            }

            object existsPending = _db.ExecuteScalar(
                "SELECT COUNT(*) FROM PendingRegistrations WHERE EmployeeId = @id AND Status = 'Pending'",
                new SqlParameter("@id", empId));
            if (Convert.ToInt32(existsPending) > 0)
            {
                LoadNextEmployeeId();
                errEmpId.Text = "ID conflict detected. A new ID has been assigned, please try again.";
                return;
            }

            int n = _db.Execute(
                "INSERT INTO PendingRegistrations " +
                "(FullName, EmployeeId, RequestedRole, Password, Department, Rank, BasicSalary, DateOfBirth, JoiningDate) " +
                "VALUES (@name, @id, @role, @pass, @dept, @rank, @sal, @dob, @join)",
                new SqlParameter("@name", name),
                new SqlParameter("@id", empId),
                new SqlParameter("@role", role),
                new SqlParameter("@pass", pass),
                new SqlParameter("@dept", dept),
                new SqlParameter("@rank", rank),
                new SqlParameter("@sal", salaryVal),
                new SqlParameter("@dob", dtpDob.Value.Date),
                new SqlParameter("@join", dtpJoining.Value.Date));

            if (n > 0)
            {
                string approvalNote = (role == "System Admin")
                    ? "Because you requested the System Admin role, only an existing System Admin can approve this request."
                    : "A System Admin or Pension Admin will review your request.";

                MessageBox.Show(
                    "Registration request submitted successfully!\n\n" +
                    "Your assigned Employee ID is: " + empId + "\n\n" +
                    approvalNote + "\n\n" +
                    "Once approved, log in using your Employee ID and Password.",
                    "Request Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                new LoginForm().Show();
                this.Close();
            }
        }
    }
}