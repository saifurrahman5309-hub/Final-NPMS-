using System;
using System.Drawing;
using System.Windows.Forms;
using PensionMgmt.Services;

namespace PensionMgmt.Forms
{
    public class PensionCalcPanel : UserControl
    {
        private TextBox txtEmpId, txtBasicSalary;
        private ComboBox cmbRank;
        private DateTimePicker dtpBirth, dtpJoining;
        private Label lblMonthly, lblLumpSum, lblDetails;

        public PensionCalcPanel()
        {
            this.BackColor = Color.FromArgb(245, 247, 250);
            BuildUI();
        }

        private void BuildUI()
        {
            Label lHead = new Label
            {
                Text      = "Pension Calculator",
                Font      = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 45, 80),
                Bounds    = new Rectangle(20, 16, 400, 34)
            };
            this.Controls.Add(lHead);

            Panel card = new Panel { BackColor = Color.White, Bounds = new Rectangle(20, 62, 500, 370) };

            int col = 20, lw = 160, tw = 200, gap = 40, y = 16;

            AddRow(card, "Employee ID (optional)", col, y, lw, tw, out txtEmpId); y += gap;
            AddRow(card, "Basic Salary (BDT) *",  col, y, lw, tw, out txtBasicSalary); y += gap;

            Label lRank = new Label { Text = "Rank *", Bounds = new Rectangle(col, y, lw, 28), TextAlign = ContentAlignment.MiddleLeft };
            cmbRank = new ComboBox { Bounds = new Rectangle(col + lw + 4, y, tw, 28), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRank.Items.AddRange(new[] { "Secretary", "Joint Secretary", "Deputy Secretary", "Senior Assistant", "Assistant" });
            cmbRank.SelectedIndex = 4;
            card.Controls.Add(lRank); card.Controls.Add(cmbRank); y += gap;

            Label lBirth = new Label { Text = "Date of Birth *", Bounds = new Rectangle(col, y, lw, 28), TextAlign = ContentAlignment.MiddleLeft };
            dtpBirth = new DateTimePicker { Bounds = new Rectangle(col + lw + 4, y, tw, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(-60) };
            card.Controls.Add(lBirth); card.Controls.Add(dtpBirth); y += gap;

            Label lJoin = new Label { Text = "Joining Date *", Bounds = new Rectangle(col, y, lw, 28), TextAlign = ContentAlignment.MiddleLeft };
            dtpJoining = new DateTimePicker { Bounds = new Rectangle(col + lw + 4, y, tw, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddYears(-30) };
            card.Controls.Add(lJoin); card.Controls.Add(dtpJoining); y += gap;

            Button btnCalc = new Button
            {
                Text      = "Calculate Pension",
                Bounds    = new Rectangle(col, y, 180, 32),
                BackColor = Color.FromArgb(25, 45, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnCalc.FlatAppearance.BorderSize = 0;
            btnCalc.Click += BtnCalc_Click;
            card.Controls.Add(btnCalc); y += 46;

            // Results
            Panel pnlResult = new Panel { BackColor = Color.FromArgb(240, 248, 255), Bounds = new Rectangle(col, y, 460, 80), Visible = false, Name = "pnlResult" };

            lblMonthly = new Label { Text = "Monthly Pension: —", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.DarkGreen, Bounds = new Rectangle(10, 8, 440, 26) };
            lblLumpSum = new Label { Text = "Lump Sum Gratuity: —", Font = new Font("Segoe UI", 11f, FontStyle.Bold), ForeColor = Color.DarkSlateBlue, Bounds = new Rectangle(10, 36, 440, 26) };
            pnlResult.Controls.Add(lblMonthly);
            pnlResult.Controls.Add(lblLumpSum);

            card.Controls.Add(pnlResult);
            y += 88;

            lblDetails = new Label { Text = "", ForeColor = Color.Gray, Bounds = new Rectangle(col, y, 460, 50), AutoSize = false };
            card.Controls.Add(lblDetails);

            this.Controls.Add(card);
        }

        private void AddRow(Panel parent, string label, int x, int y, int lw, int tw, out TextBox txt)
        {
            Label l = new Label { Text = label, Bounds = new Rectangle(x, y, lw, 28), TextAlign = ContentAlignment.MiddleLeft };
            txt = new TextBox { Bounds = new Rectangle(x + lw + 4, y, tw, 28) };
            parent.Controls.Add(l); parent.Controls.Add(txt);
        }

        private void BtnCalc_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtBasicSalary.Text.Trim(), out decimal salary) || salary <= 0)
            {
                MessageBox.Show("Enter a valid Basic Salary.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = PensionCalculator.Calculate(
                salary,
                dtpBirth.Value.Date,
                dtpJoining.Value.Date,
                cmbRank.Text,
                out decimal monthly,
                out decimal lump,
                out string message);

            Panel pnlResult = (Panel)this.Controls.Find("pnlResult", true)[0];

            if (ok)
            {
                lblMonthly.Text = $"Monthly Pension:    BDT {monthly:N2}";
                lblLumpSum.Text = $"Lump Sum Gratuity:  BDT {lump:N2}";
                pnlResult.Visible = true;
                lblDetails.Text = message;
            }
            else
            {
                pnlResult.Visible = false;
                lblDetails.Text   = message;
                MessageBox.Show(message, "Not Eligible", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
