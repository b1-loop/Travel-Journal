using System;
using System.Linq;

namespace Travel_Journal
{
    public class Account
    {
        public string UserName { get; set; } = string.Empty; // Användarnamn
        public string Password { get; set; } = string.Empty; // Lösenord
        public string RecoveryCode { get; set; } = string.Empty; // Återställningskod
        public DateTime CreatedAt { get; set; } = default; // Tid kontot skapades
        public decimal Savings { get; set; } = 0m; // Sparkonto

        // === 🔑 Validerar lösenord enligt regler ===
        public bool CheckPassword(string passWord)
        {
            bool longEnough = passWord.Length >= 6;
            bool hasNumber = passWord.Any(char.IsDigit);
            bool hasUpper = passWord.Any(char.IsUpper);
            bool hasLower = passWord.Any(char.IsLower);
            bool hasSpecial = passWord.Any(c => !char.IsLetterOrDigit(c));

            return longEnough && hasNumber && hasUpper && hasLower && hasSpecial;
        }

        // === 👤 Validerar användarnamn ===
        public bool CheckUserName(string userName)
        {
            return !string.IsNullOrWhiteSpace(userName);
        }
    }
}
