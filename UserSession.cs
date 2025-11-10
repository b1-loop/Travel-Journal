using Spectre.Console;
using System;
using System.Threading.Tasks;

namespace Travel_Journal
{
    /// <summary>
    /// Hanterar vad som händer när en användare är inloggad:
    /// - visa profil
    /// - hantera trips
    /// - budget
    /// - statistik
    /// - AI Travel Assistant
    /// - logga ut
    /// </summary>
    public class UserSession
    {
        private readonly Account _account;
        private readonly TripService _tripService;

        public UserSession(Account account)
        {
            _account = account;
            _tripService = new TripService(account.UserName);
        }

        // 🧭 Startar menyn för inloggad användare (nu asynkron för AI)
        public async Task Start()
        {
            while (true)
            {
                var sub = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title($"[bold cyan]Welcome, {_account.UserName}![/] Choose an option:")
                        .HighlightStyle(new Style(Color.Cyan1))
                        .AddChoices(
                            "👤 View Profile",
                            "➕ Add Upcoming Trip",
                            "🧳 Add Previous Trip",
                            "📋 Show All Trips",
                            "💰 Budget & Savings",
                            "📊 Statistics",
                            "🔄 Update/Change Trips",
                            "AI Travel Assistant 🤖✈️",
                            "🚪 Log out"
                        )
                );

                if (sub == "👤 View Profile")
                {
                    ShowProfile();
                    Pause();
                }
                else if (sub == "➕ Add Upcoming Trip")
                {
                    _tripService.AddUpcomingTrip();
                    Pause();
                }
                else if (sub == "🧳 Add Previous Trip")
                {
                    _tripService.AddPreviousTrip();
                    Pause();
                }
                else if (sub == "📋 Show All Trips")
                {
                    _tripService.ShowAllTrips();
                    Pause();
                }
                else if (sub == "💰 Budget & Savings")
                {
                    var budgetService = new BudgetService(_account, _tripService);
                    budgetService.ShowBudgetMenu();
                }
                else if (sub == "📊 Statistics")
                {
                    var statsService = new Statistics(_tripService);
                    statsService.StatsMenu();
                    Pause();
                }
                else if (sub == "🔄 Update/Change Trips")
                {
                    var trips = _tripService.GetTrips();
                    _tripService.UpdateTrips(trips);
                    Pause();
                }
                else if (sub == "AI Travel Assistant 🤖✈️")
                {
                    await ShowAISuggestionAsync(); // ✅ Asynkront AI-anrop
                    Pause();
                }
                else if (sub == "🚪 Log out")
                {
                    UI.Transition("Logging out...");
                    UI.Info($"Goodbye, {_account.UserName}! 👋");
                    return;
                }
            }
        }

        // 👤 Visar profilinfo
        private void ShowProfile()
        {
            var t = new Table()
                .Border(TableBorder.Rounded)
                .BorderStyle(new Style(Color.Grey50));

            t.AddColumn("Field");
            t.AddColumn("Value");

            t.AddRow("Username", _account.UserName);
            t.AddRow("Created", _account.CreatedAt == default ? "—" : _account.CreatedAt.ToString("yyyy-MM-dd HH:mm"));
            t.AddRow("Recovery Code", _account.RecoveryCode);
            t.AddRow("Savings", $"{_account.Savings} kr");

            AnsiConsole.Write(t);
        }

        // 🤖 AI Travel Assistant – genererar reseförslag via OpenAI
        public async Task ShowAISuggestionAsync()
        {
            UI.Transition("AI Travel Assistant 🤖✈️");

            // Fråga användaren om resepreferenser
            var budget = AnsiConsole.Ask<decimal>("What is your [green]budget (SEK)[/]?");
            var type = AnsiConsole.Ask<string>("What kind of [blue]trip[/] do you want? (e.g. city, beach, adventure, culture)");
            var days = AnsiConsole.Ask<int>("How many [yellow]days[/] do you plan to travel?");

            // Skapa AI-klassen och hämta förslag
            var ai = new AITravelAssistant();
            string suggestion = await ai.GetTravelSuggestionAsync(budget, type, days);

            // Visa resultatet snyggt i terminalen
            var panel = new Panel($"[white]{suggestion}[/]")
            {
                Header = new PanelHeader("🌍 Your AI Travel Suggestion"),
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1)
            };
            AnsiConsole.Write(panel);
        }

        // ⏸️ Enkel paus innan nästa meny
        private void Pause()
        {
            AnsiConsole.MarkupLine("\n[grey]Press [bold]ENTER[/] to continue...[/]");
            Console.ReadLine();
        }
    }
}
