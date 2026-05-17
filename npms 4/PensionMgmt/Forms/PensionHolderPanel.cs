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
    public class PensionHolderPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();

        private Panel pnlEmployee;
        private Panel pnlPension;

        private Label lblEmpId, lblName, lblDept, lblRank, lblSalary, lblDob, lblJoining, lblStatus;
        private Label lblMonthly, lblLumpSum, lblPensionDetails;
        private Button btnApply;

        // Store calculated values for use in Apply button
        private decimal _calculatedMonthly;
        private decimal _calculatedLump;
        private string _linkedEmpId;
        private string _linkedEmpName;

        public PensionHolderPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadMyInfo();
        }

        private void BuildUI()
        {
            Label lHeader = new Label
            {
                Text = "My Pension Information",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(20, 16, 500, 34)
            };
            this.Controls.Add(lHeader);

            Label lSub = new Label
            {
                Text = "Your employee record and pension estimate based on current data.",
                ForeColor = Color.Gray,
                Bounds = new Rectangle(20, 52, 600, 22)
            };
            this.Controls.Add(lSub);

            // Employee record card
            pnlEmployee = new Panel
            {
                BackColor = Color.White,
                Bounds = new Rectangle(20, 86, 500, 230)
            };

            Label lEmpHead = new Label
            {
                Text = "Employee Record",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(14, 10, 460, 26)
            };
            pnlEmployee.Controls.Add(lEmpHead);

            int y = 44; int lw = 160; int vx = 180;
            lblEmpId = AddInfoRow(pnlEmployee, "Employee ID:", y, lw, vx); y += 32;
            lblName = AddInfoRow(pnlEmployee, "Full Name:", y, lw, vx); y += 32;
            lblDept = AddInfoRow(pnlEmployee, "Department:", y, lw, vx); y += 32;
            lblRank = AddInfoRow(pnlEmployee, "Rank:", y, lw, vx); y += 32;
            lblSalary = AddInfoRow(pnlEmployee, "Basic Salary:", y, lw, vx); y += 32;
            lblDob = AddInfoRow(pnlEmployee, "Date of Birth:", y, lw, vx);

            int y2 = 44;
            lblJoining = AddInfoRow(pnlEmployee, "Joining Date:", y2, lw, vx + 260); y2 += 32;
            lblStatus = AddInfoRow(pnlEmployee, "Status:", y2, lw, vx + 260);

            this.Controls.Add(pnlEmployee);

            // Pension estimate card
            pnlPension = new Panel
            {
                BackColor = Color.White,
                Bounds = new Rectangle(540, 86, 420, 230)
            };

            Label lPenHead = new Label
            {
                Text = "Pension Estimate",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds = new Rectangle(14, 10, 392, 26)
            };
            pnlPension.Controls.Add(lPenHead);

            Label lNote = new Label
            {
                Text = "Based on your current record:",
                ForeColor = Color.Gray,
                Bounds = new Rectangle(14, 40, 392, 20)
            };
            pnlPension.Controls.Add(lNote);

            lblMonthly = new Label
            {
                Text = "Monthly Pension:  --",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                AutoSize = false,
                Bounds = new Rectangle(14, 68, 392, 30)
            };
            pnlPension.Controls.Add(lblMonthly);

            lblLumpSum = new Label
            {
                Text = "Lump Sum Gratuity:  --",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                AutoSize = false,
                Bounds = new Rectangle(14, 106, 392, 30)
            };
            pnlPension.Controls.Add(lblLumpSum);

            lblPensionDetails = new Label
            {
                Text = "",
                ForeColor = Color.DimGray,
                AutoSize = false,
                Bounds = new Rectangle(14, 144, 392, 50)
            };
            pnlPension.Controls.Add(lblPensionDetails);

            // Apply for Payout button — hidden by default
            btnApply = new Button
            {
                Text = "✔ Apply for Payout",
                Bounds = new Rectangle(14, 196, 180, 34),
                BackColor = Color.FromArgb(0, 130, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Visible = false
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += BtnApply_Click;
            pnlPension.Controls.Add(btnApply);

            this.Controls.Add(pnlPension);
        }

        private Label AddInfoRow(Panel parent, string labelText, int y, int labelWidth, int valueX)
        {
            Label lbl = new Label
            {
                Text = labelText,
                ForeColor = Color.DimGray,
                Bounds = new Rectangle(14, y, labelWidth, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label val = new Label
            {
                Text = "--",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Bounds = new Rectangle(valueX, y, 160, 26),
                TextAlign = ContentAlignment.MiddleLeft
            };
            parent.Controls.Add(lbl);
            parent.Controls.Add(val);
            return val;
        }

        private void LoadMyInfo()
        {
            string myId = CurrentUser.Username;

            // Look up linked employee ID from PensionHolder table
            object linkedId = _db.ExecuteScalar(
                "SELECT EmployeeId FROM PensionHolder WHERE UserName = @u",
                new SqlParameter("@u", myId));

            string empId = (linkedId != null && linkedId != DBNull.Value && !string.IsNullOrEmpty(linkedId.ToString()))
                ? linkedId.ToString()
                : myId;

            _linkedEmpId = empId;

            DataTable dt = _db.GetTable(
                "SELECT EmployeeId, FullName, Department, Rank, BasicSalary, DateOfBirth, JoiningDate, Status " +
                "FROM Employees WHERE EmployeeId = @id",
                new SqlParameter("@id", empId));

            if (dt.Rows.Count == 0)
            {
                lblEmpId.Text = myId;
                lblName.Text = CurrentUser.FullName ?? "--";
                lblDept.Text = "Not linked to an employee record";
                lblRank.Text = "--";
                lblSalary.Text = "--";
                lblDob.Text = "--";
                lblJoining.Text = "--";
                lblStatus.Text = "--";
                lblMonthly.Text = "No employee record found.";
                lblLumpSum.Text = "";
                lblPensionDetails.Text = "Please ask a System Admin to link your account to an employee record.";
                btnApply.Visible = false;
                return;
            }

            DataRow row = dt.Rows[0];
            decimal salary = Convert.ToDecimal(row["BasicSalary"]);
            DateTime dob = Convert.ToDateTime(row["DateOfBirth"]);
            DateTime doj = Convert.ToDateTime(row["JoiningDate"]);
            string rank = row["Rank"].ToString();
            string empName = row["FullName"].ToString();

            _linkedEmpName = empName;

            lblEmpId.Text = row["EmployeeId"].ToString();
            lblName.Text = empName;
            lblDept.Text = row["Department"].ToString();
            lblRank.Text = rank;
            lblSalary.Text = "BDT " + salary.ToString("N2");
            lblDob.Text = dob.ToString("dd MMM yyyy");
            lblJoining.Text = doj.ToString("dd MMM yyyy");
            lblStatus.Text = row["Status"].ToString();

            // Calculate pension estimate
            bool ok = PensionCalculator.Calculate(
                salary, dob, doj, rank,
                out decimal monthly, out decimal lump, out string msg);

            if (ok)
            {
                _calculatedMonthly = monthly;
                _calculatedLump = lump;

                lblMonthly.ForeColor = Color.DarkGreen;
                lblMonthly.Text = "Monthly Pension:  BDT " + monthly.ToString("N2");
                lblLumpSum.Text = "Lump Sum Gratuity:  BDT " + lump.ToString("N2");
                lblPensionDetails.Text = msg;

                // Only show Apply button if no payout record already exists for this employee
                object existingCount = _db.ExecuteScalar(
                    "SELECT COUNT(*) FROM Payouts WHERE EmployeeId = @id",
                    new SqlParameter("@id", empId));

                if (Convert.ToInt32(existingCount) == 0)
                {
                    btnApply.Visible = true;
                    btnApply.Text = "✔ Apply for Payout";
                    btnApply.Enabled = true;
                }
                else
                {
                    // Payout already exists — show status instead of button
                    object payoutStatus = _db.ExecuteScalar(
                        "SELECT TOP 1 Status FROM Payouts WHERE EmployeeId = @id ORDER BY ProcessedDate DESC",
                        new SqlParameter("@id", empId));

                    string status = payoutStatus?.ToString() ?? "Pending";
                    btnApply.Visible = true;
                    btnApply.Enabled = false;
                    btnApply.Text = "Payout Status: " + status;
                    btnApply.BackColor = status == "Approved"
                        ? Color.FromArgb(25, 45, 80)
                        : Color.DimGray;
                }
            }
            else
            {
                lblMonthly.ForeColor = Color.OrangeRed;
                lblMonthly.Text = "Not yet eligible for pension.";
                lblLumpSum.Text = "";
                lblPensionDetails.Text = msg;
                btnApply.Visible = false;
            }
        }

        private void BtnApply_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                string.Format(
                    "Apply for pension payout?\n\n" +
                    "Monthly Pension : BDT {0:N2}\n" +
                    "Lump Sum        : BDT {1:N2}\n\n" +
                    "Your application will be reviewed by an admin.",
                    _calculatedMonthly, _calculatedLump),
                "Confirm Application", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            int n = _db.Execute(
                "INSERT INTO Payouts (EmployeeId, FullName, MonthlyPension, LumpSum, ProcessedDate, Notes, Status) " +
                "VALUES (@id, @name, @m, @l, @d, @notes, 'Pending')",
                new SqlParameter("@id", _linkedEmpId),
                new SqlParameter("@name", _linkedEmpName),
                new SqlParameter("@m", _calculatedMonthly),
                new SqlParameter("@l", _calculatedLump),
                new SqlParameter("@d", DateTime.Today),
                new SqlParameter("@notes", "Applied by pension holder. Awaiting admin approval."));

            if (n > 0)
            {
                MessageBox.Show(
                    "Your payout application has been submitted successfully.\n" +
                    "It is now Pending admin approval.",
                    "Application Submitted", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reload to update button state
                LoadMyInfo();
            }
        }
    }
}