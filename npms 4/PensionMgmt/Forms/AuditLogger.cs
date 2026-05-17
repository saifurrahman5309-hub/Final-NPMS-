using System;
using System.Data.SqlClient;
using PensionMgmt.Data;
using PensionMgmt.Session;

namespace PensionMgmt.Services
{
    /// <summary>
    /// Static helper used by every panel to write a single audit log entry.
    /// Call AuditLogger.Log(...) after any significant action.
    /// </summary>
    public static class AuditLogger
    {
        private static readonly DbAccess _db = new DbAccess();

        /// <summary>
        /// Writes one audit log entry to the AuditLog table.
        /// </summary>
        /// <param name="actionType">Short category, e.g. "Employee Added"</param>
        /// <param name="targetId">The Employee ID or user ID being acted on</param>
        /// <param name="targetName">Full name of the person being acted on</param>
        /// <param name="description">Human-readable detail of what happened</param>
        public static void Log(string actionType, string targetId, string targetName, string description)
        {
            try
            {
                string performedBy = CurrentUser.Username ?? "System";

                _db.Execute(
                    "INSERT INTO AuditLog (ActionType, TargetId, TargetName, Description, PerformedBy, PerformedAt) " +
                    "VALUES (@action, @tid, @tname, @desc, @by, @at)",
                    new SqlParameter("@action", actionType),
                    new SqlParameter("@tid", targetId ?? ""),
                    new SqlParameter("@tname", targetName ?? ""),
                    new SqlParameter("@desc", description),
                    new SqlParameter("@by", performedBy),
                    new SqlParameter("@at", DateTime.Now));
            }
            catch
            {
                // Never let audit logging crash the main application
            }
        }
    }
}