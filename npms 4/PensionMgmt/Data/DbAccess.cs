using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using PensionMgmt.Session;

namespace PensionMgmt.Data
{
    public class DbAccess
    {
        private const string CONNECTION_STRING =
            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=newdbnpms6;Integrated Security=True;TrustServerCertificate=True";

        private SqlConnection GetConnection()
        {
            return new SqlConnection(CONNECTION_STRING);
        }

        public DataTable GetTable(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n" + ex.Message, "DB Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
        }

        public int Execute(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n" + ex.Message, "DB Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (SqlConnection con = GetConnection())
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    return cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database error:\n" + ex.Message, "DB Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public bool TestConnection()
        {
            try
            {
                using (SqlConnection con = GetConnection())
                {
                    con.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Logs an action to the AuditLog table using the current session user.
        /// Call this after any significant create/update/delete operation.
        /// </summary>
        public void LogAction(string action)
        {
            try
            {
                string role = CurrentUser.Role.ToString()
                    .Replace("SystemAdmin", "System Admin")
                    .Replace("PensionAdmin", "Pension Admin")
                    .Replace("PensionManager", "Pension Manager")
                    .Replace("PensionHolder", "Pension Holder");

                Execute(
                    "INSERT INTO AuditLog (EmployeeId, FullName, Role, Action) " +
                    "VALUES (@id, @name, @role, @action)",
                    new SqlParameter("@id", CurrentUser.Username ?? "unknown"),
                    new SqlParameter("@name", CurrentUser.FullName ?? CurrentUser.Username ?? "unknown"),
                    new SqlParameter("@role", role),
                    new SqlParameter("@action", action));
            }
            catch
            {
                // Logging should never crash the app
            }
        }
    }
}