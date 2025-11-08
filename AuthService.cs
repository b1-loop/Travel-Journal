using Spectre.Console;
using System;

namespace Travel_Journal
{
    public class AuthService
    {
        // Registrerar ett nytt konto
        public bool Register(string username, string password)
        {
            // Kolla om användaren redan finns
            if (AccountStore.Exists(username))
            {
                UI.Warn("User already exists!");
                return false;
            }

            // Skapa nytt konto
            var acc = new Account();

            // Kontrollera att användarnamnet är giltigt
            if (!acc.CheckUserName(username))
            {
                UI.Error("Username must be at least 1 character long.");
                return false;
            }

            // Kontrollera att lösenordet uppfyller kraven
            if (!acc.CheckPassword(password))
            {
                UI.Error("Password requirements: min 6 chars, uppercase, lowercase, number, special.");
                return false;
            }

            // Sätt grunddata för kontot
            acc.UserName = username;
            acc.Password = password;
            acc.RecoveryCode = Util.GenerateRecoveryCode(); // Skapar återställningskod
            acc.CreatedAt = DateTime.UtcNow; // Sparar när kontot skapades

            // Spara till filen users.json
            UI.WithStatus("Saving account...", () =>
            {
                AccountStore.Add(acc);
                AccountStore.Save();
            });

            // Bekräftelse till användaren
            var panel = new Panel($"""
            [green]✅ User {acc.UserName} created successfully![/]
            [yellow]Recovery code:[/] [bold]{acc.RecoveryCode}[/]
            """)
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("Registration Successful", Justify.Center)
            };
            AnsiConsole.Write(panel);

            return true;
        }

        // Loggar in en användare
        public Account? Login(string username, string password)
        {
            // Hämta kontot från AccountStore
            var acc = AccountStore.Get(username);

            if (acc == null)
            {
                UI.Error("Unknown username.");
                return null;
            }

            // Kolla om lösenordet stämmer
            if (acc.UserName == username && acc.Password == password)
            {
                UI.Success($"Logged in as [bold]{username}[/]! ✈️");
                return acc;
            }

            UI.Error("Wrong username or password.");
            return null;
        }

        // Återställer ett glömt lösenord
        public void ResetPassword(string username, string recoveryCode, string newPassword, string confirmPassword)
        {
            // Kontrollera att båda lösenorden är samma
            if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
            {
                UI.Error("Passwords do not match.");
                return;
            }

            // Hämta kontot
            var acc = AccountStore.Get(username);
            if (acc == null)
            {
                UI.Error("Unknown username.");
                return;
            }

            // Kontrollera återställningskoden
            if (!string.Equals(acc.RecoveryCode, recoveryCode, StringComparison.Ordinal))
            {
                UI.Error("Wrong recovery code.");
                return;
            }

            // Kontrollera att nya lösenordet uppfyller kraven
            if (!acc.CheckPassword(newPassword))
            {
                UI.Error("New password does not meet the requirements.");
                return;
            }

            // Uppdatera lösenordet och skapa ny återställningskod
            UI.WithStatus("Updating password...", () =>
            {
                acc.Password = newPassword;
                acc.RecoveryCode = Util.GenerateRecoveryCode();
                AccountStore.Update(acc);
                AccountStore.Save();
            });

            // Bekräftelse till användaren
            UI.Success("Password reset! A new recovery code has been generated.");
        }
    }
}
