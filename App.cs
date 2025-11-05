using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;
using System.IO;
using System.Text.Json;
using Spectre.Console.Cli;

namespace Travel_Journal
{
    public static class App
    {
        public static void Run()
        {
            UI.Splash();
            AccountStore.LoadWithProgress();

            while (true)
            {
                var choice = UI.MainMenu();

                if (choice == "Register")
                    Register();
                else if (choice == "Login")
                    Login();
                else if (choice == "Forgot password")
                    ForgotPassword();

                else
                {
                    UI.Transition("Avslutar...");
                    AnsiConsole.MarkupLine("[green]Tack för att du använde systemet![/]");
                    AnsiConsole.MarkupLine("[grey]Ha en fantastisk dag! [/]");
                    System.Threading.Thread.Sleep(1200);
                    return;
                }
            }

        }

        // -------- Register --------
        private static void Register()
        {
            UI.Transition("Registrera konto");

            var username = AnsiConsole.Ask<string>("Användarnamn:");
            if (AccountStore.Exists(username))
            {
                UI.Warn("Användare finns redan!");
                return;
            }

            var password = AnsiConsole.Prompt(new TextPrompt<string>("Lösenord:").Secret());

            var acc = new Account();
            if (!acc.Register(username, password))
            {
                UI.Error("Lösenord krav: min 6, stor bokstav, liten, siffra, special");
                return;
            }

            // Skapa recovery-kod + createdAt
            acc.RecoveryCode = Util.GenerateRecoveryCode();
            acc.CreatedAt = DateTime.UtcNow;

            UI.WithStatus("Sparar konto...", () =>
            {
                AccountStore.Add(acc);
                AccountStore.Save();
            });

            // Visa recovery-kod tydligt
            var grid = new Grid();
            grid.AddColumn();
            grid.AddRow(new Markup($"Användare [bold]{acc.UserName}[/] skapad."));
            grid.AddRow(new Markup("[yellow]VIKTIGT:[/] Spara din [bold]återställningskod[/] för glömt lösenord!"));
            grid.AddRow(new Markup($"[bold green]{acc.RecoveryCode}[/]"));

            var panel = new Panel(grid)
            {
                Border = BoxBorder.Rounded,
                Header = new PanelHeader("Registrering OK", Justify.Center)
            };
            AnsiConsole.Write(panel);
        }

        // -------- Login --------
        private static void Login()
        {
            UI.Transition("Logga in");

            var username = AnsiConsole.Ask<string>("Användarnamn:");
            var password = AnsiConsole.Prompt(new TextPrompt<string>("Lösenord:").Secret());

            var acc = AccountStore.Get(username);
            if (acc == null)
            {
                UI.Error("Okänt användarnamn");
                return;
            }

            var ok = false;
            UI.WithStatus("Verifierar...", () =>
            {
                ok = acc.Login(username, password);
                System.Threading.Thread.Sleep(350);
            });

            if (!ok)
            {
                UI.Error("Fel användare eller lösenord");
                return;
            }

            UI.Success($"Inloggad som {username}!");

            // Enkel profilmeny
            while (true)
            {
                var sub = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold cyan]Meny[/]")
                        .HighlightStyle(new Style(Color.Chartreuse1))
                        .AddChoices("Visa profil", "Logga ut"));

                if (sub == "Visa profil")
                {
                    var t = new Table();
                    t.Border = TableBorder.Rounded;
                    t.BorderStyle = new Style(Color.Grey50);
                    t.AddColumn("Fält");
                    t.AddColumn("Värde");
                    t.AddRow("Användarnamn", acc.UserName);
                    t.AddRow("Skapad (UTC)", acc.CreatedAt == default ? "—" : acc.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
                    t.AddRow("Recovery-kod", acc.RecoveryCode);
                    AnsiConsole.Write(t);
                }
                else break; // Logga ut
            }
        }

        // -------- Forgot Password --------
        private static void ForgotPassword()
        {
            UI.Transition("Glömt lösenord");

            var username = AnsiConsole.Ask<string>("Användarnamn:");
            var code = AnsiConsole.Ask<string>("Återställningskod:");
            var newPwd = AnsiConsole.Prompt(new TextPrompt<string>("Nytt lösenord:").Secret());
            var confirm = AnsiConsole.Prompt(new TextPrompt<string>("Bekräfta nytt lösenord:").Secret());

            if (!string.Equals(newPwd, confirm, StringComparison.Ordinal))
            {
                UI.Error("Lösenorden matchar inte");
                return;
            }

            var acc = AccountStore.Get(username);
            if (acc == null)
            {
                UI.Error("Okänt användarnamn");
                return;
            }

            if (!string.Equals(acc.RecoveryCode, code, StringComparison.Ordinal))
            {
                UI.Error("Fel återställningskod");
                return;
            }

            if (!acc.CheckPassword(newPwd))
            {
                UI.Error("Nytt lösenord uppfyller inte kraven");
                return;
            }

            UI.WithStatus("Uppdaterar lösenord...", () =>
            {
                acc.Password = newPwd;
                acc.RecoveryCode = Util.GenerateRecoveryCode(); // rotera koden
                AccountStore.Update(acc);
                AccountStore.Save();
            });

            UI.Success("Lösenord återställt! Ny återställningskod skapad.");
        }
    }
}

