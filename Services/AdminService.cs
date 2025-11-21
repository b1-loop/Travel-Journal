using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Journal.Data;
using Travel_Journal.Models;
using Travel_Journal.UIServices;

namespace Travel_Journal.Services
{
    public class AdminService
    {
        private readonly DataStore<Account> _accountStore;
        public AdminService(DataStore<Account> accountStore)
        {
            _accountStore = accountStore;
        }

        public void ShowAllUsers()
        {
            var accounts = _accountStore.GetAll();

            if (!accounts.Any())
            {
                UI.Warn("No accounts found.");
                UI.Pause();
                return;
            }

            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Grey50)
                .Centered();

            table.AddColumn("[cyan]Email[/]");
            table.AddColumn("[cyan]Name[/]");
            table.AddColumn("[cyan]Phone[/]");
            table.AddColumn("[cyan]Admin[/]");

            foreach (var acc in accounts)
            {
                table.AddRow(
                    acc.Email ?? "-",
                    acc.IsAdmin ? "[green]Yes[/]" : "[grey]No[/]"
                );
            }

            AnsiConsole.Write(table);
            UI.Pause();
        }
        public void DeleteUser()
        {
            var email = AnsiConsole.Ask<string>("[red]Enter email of user to delete:[/]");

            var accounts = _accountStore.GetAll();
            var account = accounts.FirstOrDefault(a => a.Email == email);

            if (account == null)
            {
                UI.Error("User not found.");
                UI.Pause();
                return;
            }

            if (account.IsAdmin)
            {
                UI.Error("You cannot delete an admin account from here (for safety).");
                UI.Pause();
                return;
            }

            var confirm = AnsiConsole.Confirm($"Are you sure you want to delete [yellow]{email}[/]?");
            if (!confirm) return;

            accounts.Remove(account);
            _accountStore.Save(accounts);

            UI.Success("User deleted successfully.");
            UI.Pause();
        }
        public void ToggleAdmin()
        {
            var email = AnsiConsole.Ask<string>("[aqua]Enter email to toggle admin role:[/]");
            var accounts = _accountStore.GetAll();
            var account = accounts.FirstOrDefault(a => a.Email == email);

            if (account == null)
            {
                UI.Error("User not found.");
                UI.Pause();
                return;
            }

            account.IsAdmin = !account.IsAdmin;
            _accountStore.Save(accounts);

            var status = account.IsAdmin ? "granted" : "revoked";

            UI.Success($"Admin role {status} for {email}.");
            UI.Pause();
        }
    }
}
