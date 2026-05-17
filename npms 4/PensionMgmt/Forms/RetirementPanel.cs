using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Data;

namespace PensionMgmt.Forms
{
    public class RetirementPanel : UserControl
    {
        private readonly DbAccess _db = new DbAccess();
        private DataGridView dgv;

        public RetirementPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
            LoadAll();
        }

        private void BuildUI()
        {
            Label lHead = new Label
            {
                Text      = "Retirement Management",
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds    = new Rectangle(20, 16, 400, 34)
            };
            this.Controls.Add(lHead);

            // Filter buttons
            Button btnAll = MakeBtn("All Employees",    20, 60, Color.FromArgb(25, 45, 80));
            Button btnThis = MakeBtn("Retiring This Year", 180, 60, Color.OrangeRed);
            Button btnArchived = MakeBtn("Already Retired",  360, 60, Color.SlateGray);
            btnAll.Click      += (s, e) => LoadAll();
            btnThis.Click     += (s, e) => LoadRetiringThisYear();
            btnArchived.Click += (s, e) => LoadArchived();
            this.Controls.Add(btnAll);
            this.Controls.Add(btnThis);
            this.Controls.Add(btnArchived);

            dgv = new DataGridView
            {
                Bounds              = new Rectangle(20, 104, this.Width - 40, 400),
                Anchor              = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly            = true,
                AllowUserToAddRows  = false,
                SelectionMode       = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor     = Color.White,
                BorderStyle         = BorderStyle.None,
                RowHeadersVisible   = false
            };
            this.Controls.Add(dgv);
        }

        private Button MakeBtn(string text, int x, int y, Color bg)
        {
            Button b = new Button
            {
                Text      = text,
                Bounds    = new Rectangle(x, y, 160, 32),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private void LoadAll()
        {
            dgv.DataSource = _db.GetTable(
                "SELECT EmployeeId, FullName, Department, Rank, BasicSalary, JoiningDate, Status " +
                "FROM Employees ORDER BY JoiningDate");
        }

        private void LoadRetiringThisYear()
        {
            dgv.DataSource = _db.GetTable(
                "SELECT EmployeeId, FullName, Department, Rank, BasicSalary, JoiningDate, Status " +
                "FROM Employees " +
                "WHERE Status = 'Active' " +
                // Completes 30 service years sometime this year
                "AND YEAR(DATEADD(YEAR,30,JoiningDate)) = YEAR(GETDATE()) " +
                // AND reaches age 59 this year or already has
                "AND YEAR(DATEADD(YEAR,59,DateOfBirth)) <= YEAR(GETDATE()) " +
                "ORDER BY JoiningDate");
        }

        private void LoadArchived()
        {
            dgv.DataSource = _db.GetTable(
                "SELECT EmployeeId, FullName, Department, Rank, BasicSalary, JoiningDate, Status " +
                "FROM Employees " +
                "WHERE Status = 'Retired' " +
                // Service is at least 30 years
                "OR (DATEADD(YEAR,30,JoiningDate) <= GETDATE() " +
                // AND age is at least 59
                "AND DATEADD(YEAR,59,DateOfBirth) <= GETDATE()) " +
                "ORDER BY JoiningDate");
        }
    }
}
