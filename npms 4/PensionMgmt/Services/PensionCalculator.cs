using System;

namespace PensionMgmt.Services
{
    public static class PensionCalculator
    {
        // Default pension rules (can be adjusted)
        public const int     RetirementAge    = 59;
        public const int     MinServiceYears  = 10;
        public const int     FullPensionYears = 30;
        public const decimal MaxPensionPct    = 80m; // % of basic salary

        // Rank bonus percentages
        public static int GetRankBonus(string rank)
        {
            switch (rank?.ToLower())
            {
                case "secretary":         return 10;
                case "joint secretary":   return 8;
                case "deputy secretary":  return 6;
                case "senior assistant":  return 4;
                case "assistant":         return 2;
                default:                  return 0;
            }
        }

        /// <summary>
        /// Calculates monthly pension and lump sum gratuity.
        /// Returns false + reason string if not eligible.
        /// </summary>
        public static bool Calculate(
            decimal  basicSalary,
            DateTime dateOfBirth,
            DateTime dateOfJoining,
            string   rank,
            out decimal monthlyPension,
            out decimal lumpSum,
            out string  message)
        {
            monthlyPension = 0;
            lumpSum        = 0;

            DateTime today = DateTime.Today;

            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth > today.AddYears(-age)) age--;

            int serviceYears = today.Year - dateOfJoining.Year;
            if (dateOfJoining > today.AddYears(-serviceYears)) serviceYears--;

            if (age < RetirementAge)
            {
                message = string.Format("Not eligible: current age {0} is below retirement age {1}.", age, RetirementAge);
                return false;
            }

            if (serviceYears < MinServiceYears)
            {
                message = string.Format("Not eligible: service years {0} is below minimum {1}.", serviceYears, MinServiceYears);
                return false;
            }

            int     cappedYears = Math.Min(serviceYears, FullPensionYears);
            decimal basePct     = (decimal)cappedYears / FullPensionYears * MaxPensionPct;
            decimal rankBonus   = GetRankBonus(rank);
            decimal finalPct    = Math.Min(basePct + rankBonus, MaxPensionPct);

            monthlyPension = Math.Round(basicSalary * finalPct / 100m, 2);
            lumpSum        = Math.Round(monthlyPension * 12m, 2); // one year gratuity

            message =
                string.Format("Age: {0} yrs  |  Service: {1} yrs (capped at {2})\n", age, serviceYears, cappedYears) +
                string.Format("Base rate: {0:0.##}%  +  Rank bonus ({1}): {2}%\n", basePct, rank, rankBonus) +
                string.Format("Final rate: {0:0.##}%  of Basic Salary BDT {1:N0}", finalPct, basicSalary);

            return true;
        }
    }
}
