using System;
using System.Collections.Generic;
using System.Linq;
using Spectre.Console;
using Travel_Journal.Data;
using Travel_Journal.Models;
using Travel_Journal.UIServices;

namespace Travel_Journal.Services
{
    /// <summary>
    /// TripService är programmets "Motor".
    /// Den ansvarar för att hålla koll på listan med resor (state) och prata med databasen/filen.
    /// Den innehåller INGEN kod för Spectre.Console (UI).
    /// </summary>
    public class TripService
    {
        // === Fält ===

        // Huvudlistan som håller alla resor i minnet medan programmet körs
        private List<Trip> trips = new();

        // Användarnamnet (behövs för att veta vilken fil vi ska ladda/spara)
        private readonly string username;

        // DataStore sköter själva "grovjobbet" med att läsa/skriva JSON
        private readonly DataStore<Trip> store;

        // Vi exponerar användarnamnet så att UI kan visa det i rubriker (t.ex. "All Trips for User X")
        public string UserName => username;

        // === Konstruktor ===
        public TripService(string username)
        {
            // Spara det inkommande användarnamnet i klassens fält så att det blir tillgängligt för hela klassen (inte bara i konstruktorn).
            this.username = username;

            // Koppla till json-filen: "användarnamn_trips.json"
            store = new DataStore<Trip>($"{username}_trips.json");

            // Försök ladda resor direkt när servicen startar.
            try
            {
                trips = store.Load();
            }
            catch
            {
                // Om filen inte finns eller är trasig, startar vi med en tom lista
                // så att programmet inte kraschar.
                trips = new List<Trip>();
            }
        }

        // === Data-metoder (API mot UI) ===

        // ============================================================
        // === Add Upcoming Trip (Framtida resa) =======================
        // ============================================================
        // === Add Upcoming Trip (separerat datum) ===
        public void AddUpcomingTrip()
        {
            int step = 0;

            string country = "";
            string city = "";
            decimal budget = 0;
            DateTime startDate = default;
            DateTime endDate = default;
            int passengers = 0;

            while (step < 6)
            {
                // === STEP 0 — COUNTRY ===
                if (step == 0)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var c = UI.AskWithBack("Which country are you visiting");
                    if (c == null) return;

                    country = c;
                    step++;
                }

                // === STEP 1 — CITY ===
                else if (step == 1)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var c = UI.AskStep("Which city?");
                    if (c == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    city = c;
                    step++;
                }

                // === STEP 2 — BUDGET ===
                else if (step == 2)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var b = UI.AskStepDecimal("Planned budget?");
                    if (b == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    budget = b.Value;
                    step++;
                }

                // === STEP 3 — DEPARTURE DATE ===
                else if (step == 3)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var s = UI.AskStepDate("Departure date (YYYY-MM-DD)");
                    if (s == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    startDate = s.Value;
                    step++;
                }

                // === STEP 4 — RETURN DATE ===
                else if (step == 4)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var e = UI.AskStepDate("Return date (YYYY-MM-DD)");
                    if (e == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    if (startDate > e)
                    {
                        UI.Warn("Return date must be after departure date.");
                        continue;
                    }

                    endDate = e.Value;
                    step++;
                }

                // === STEP 5 — PASSENGERS ===
                else if (step == 5)
                {
                    UI.ShowFormHeader("Add Upcoming Trip ✈️",
                        country, city, budget, startDate, endDate, passengers);

                    var p = UI.AskStepInt("How many passengers?");
                    if (p == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    passengers = p.Value;
                    step++;
                }
            }

            var newTrip = new Trip
            {
                Country = country,
                City = city,
                PlannedBudget = budget,
                StartDate = startDate,
                EndDate = endDate,
                NumberOfPassengers = passengers
            };

            trips.Add(newTrip);
            Save();

            UI.Success($"Trip to {city}, {country} added successfully!");
            UI.Pause();
        }



        // ============================================================
        // === Add Previous Trip (Genomförd resa) ======================
        // ============================================================
        // === Lägger till en tidigare resa (Previous Trip) — nu separerade datumfält ===
        public void AddPreviousTrip()
        {
            int step = 0; // Håller koll på vilket steg vi befinner oss i

            // Variabler som fylls i steg för steg
            string country = "";
            string city = "";
            decimal budget = 0;
            decimal cost = 0;
            DateTime startDate = default;
            DateTime endDate = default;
            int passengers = 0;
            int score = 0;
            string review = "";

            // Totalt 9 steg (land, stad, budget, cost, avresa, hemresa, pass, rating, review)
            while (step < 9)
            {
                // === STEG 0 — LAND ===
                if (step == 0)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var c = UI.AskWithBack("Which country did you visit");
                    if (c == null)
                        return; // back to menu

                    country = c;
                    step++;
                }

                // === STEG 1 — STAD ===
                else if (step == 1)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var c = UI.AskStep("Which city?");
                    if (c == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    city = c;
                    step++;
                }

                // === STEG 2 — PLANERAD BUDGET ===
                else if (step == 2)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var b = UI.AskStepDecimal("Planned budget?");
                    if (b == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    budget = b.Value;
                    step++;
                }

                // === STEG 3 — TOTAL KOSTNAD ===
                else if (step == 3)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var cst = UI.AskStepDecimal("Total cost?");
                    if (cst == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    cost = cst.Value;
                    step++;


                }

                // === STEG 4 — AVLÄSNING DATUM (SEPARAT) ===
                else if (step == 4)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var s = UI.AskStepDate("Departure date (YYYY-MM-DD)");
                    if (s == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    startDate = s.Value;
                    step++;
                }

                // === STEG 5 — HEMRESA DATUM (SEPARAT) ===
                else if (step == 5)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var e = UI.AskStepDate("Return date (YYYY-MM-DD)");
                    if (e == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    if (startDate > e)
                    {
                        UI.Warn("Return date must be after departure date.");
                        Logg.Log($"User '{username}' entered invalid return date in AddPreviousTrip.");
                        continue;
                    }

                    endDate = e.Value;
                    step++;
                }

                // === STEG 6 — PASSAGERARE ===
                else if (step == 6)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var p = UI.AskStepInt("How many passengers?");
                    if (p == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    passengers = p.Value;
                    step++;
                }

                // === STEG 7 — BETYG ===
                else if (step == 7)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var r = UI.AskStepDecimal("Trip rating (1–5)");
                    if (r == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    int rating = (int)r.Value;

                    if (rating < 1 || rating > 5)
                    {
                        UI.Warn("Rating must be between 1–5.");
                        Logg.Log($"User '{username}' entered invalid rating in AddPreviousTrip.");
                        continue;
                    }

                    score = rating;
                    step++;
                }

                // === STEG 8 — RECENSION ===
                else if (step == 8)
                {
                    UI.ShowFormHeader("Add Previous Trip 🧳",
                        country, city, budget, startDate, endDate, passengers,
                        cost, score, review);

                    var rv = UI.AskStep("Write a short review");
                    if (rv == null)
                    {
                        step--;
                        UI.BackOneStep();
                        continue;
                    }

                    review = rv;
                    step++;
                }
            }

            // === SKAPA & SPARA RESAN ===
            var newTrip = new Trip
            {
                Country = country,
                City = city,
                PlannedBudget = budget,
                Cost = cost,
                StartDate = startDate,
                EndDate = endDate,
                NumberOfPassengers = passengers,
                Score = score,
                Review = review
            };

            trips.Add(newTrip);
            Save();

           //Anropar showbudget metod för popup

            NotificationService.ShowBudgetStatusForTrip(newTrip);


            UI.Pause();
        }


        // === Visar alla resor i tabellform ===
        public void ShowAllTrips()
        {
            AnsiConsole.Clear();
            UI.Transition($"All Trips for {username} 🌍");

            if (!trips.Any())
            {
                UI.Warn("No trips found for this account.");
                Logg.Log($"No trips found for user '{username}' in ShowAllTrips.");
                return;
            }

            // --- 1. Skapa kategorier ---
            var categories = new List<(string Title, Color Color, List<Trip> List)>
            {
        ("Ongoing Trips",   Color.Yellow, trips.Where(t => !t.IsUpcoming && !t.IsCompleted).OrderBy(t => t.StartDate).ToList()),
        ("Upcoming Trips",  Color.Green,  trips.Where(t => t.IsUpcoming).OrderBy(t => t.StartDate).ToList()),
        ("Completed Trips", Color.Grey,   trips.Where(t => t.IsCompleted).OrderBy(t => t.StartDate).ToList())
            };

            // --- 2. Loopa och visa varje kategori ---
            foreach (var (title, color, tripList) in categories)
            {
                AnsiConsole.WriteLine();

                var rule = new Rule($"[bold {color}]{title}[/]")
                {
                    Justification = Justify.Center
                };
                AnsiConsole.Write(rule);

                if (!tripList.Any())
                {
                    AnsiConsole.MarkupLine("[grey]No trips in this category.[/]");
                    Logg.Log($"No trips found in category '{title}' for user '{username}'.");
                    continue;
                }

                var table = new Table()
                    .Centered()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Grey50);

                // Kolumner
                table.AddColumn("[bold cyan]Country[/]");
                table.AddColumn("[bold cyan]City[/]");
                table.AddColumn("[bold cyan]Dates[/]");
                table.AddColumn("[bold cyan]Budget[/]");
                table.AddColumn("[bold cyan]Cost[/]");
                table.AddColumn("[bold cyan]Status[/]");
                table.AddColumn("[bold cyan]Rating[/]");
                table.AddColumn("[bold cyan]Review[/]");
                table.AddColumn("[bold cyan]Pax[/]");

                // Fyll tabell
                foreach (var trip in tripList)
                {
                    string dateRange = $"{trip.StartDate:yyyy-MM-dd} → {trip.EndDate:yyyy-MM-dd}";
                    string budget = $"{trip.PlannedBudget}";
                    string cost = trip.Cost > 0 ? $"{trip.Cost}" : "[grey]—[/]";
                    string rating = trip.Score > 0 ? $"{trip.Score}/5" : "[grey]—[/]";
                    string review = string.IsNullOrWhiteSpace(trip.Review) ? "[grey]No review[/]" : trip.Review;
                    string passengers = $"{trip.NumberOfPassengers}";

                    string statusText = trip.IsUpcoming ? "Upcoming" :
                                        trip.IsCompleted ? "Completed" : "Ongoing";

                    table.AddRow(
                        new Markup($"[{color}]{trip.Country}[/]"),
                        new Markup($"[{color}]{trip.City}[/]"),
                        new Markup($"[{color}]{dateRange}[/]"),
                        new Markup($"[{color}]{budget}[/]"),
                        new Markup($"[{color}]{cost}[/]"),
                        new Markup($"[{color}]{statusText}[/]"),
                        new Markup($"[{color}]{rating}[/]"),
                        new Markup($"[{color}]{review}[/]"),
                        new Markup($"[{color}]{passengers}[/]")
                    );
                }

                AnsiConsole.Write(table);
            }

            // ---- Footer ----
            var footer = new Rule($"[grey]Total trips: {trips.Count}[/]")
            {
                Justification = Justify.Center
            };
            AnsiConsole.Write(footer);
        }
        public List<Trip> GetTrips() //Hjälpmetod för att hämta resor.
        {
            return trips;
        }

        /// <summary>
        /// Lägger till en ny resa i listan och sparar direkt till filen.
        /// </summary>
        public void AddTrip(Trip trip)
        {
            trips.Add(trip);
            Save(); // Viktigt: Spara direkt så ingen data går förlorad vid krasch
        }

        /// <summary>
        /// Tar bort en resa från listan och uppdaterar filen.
        /// </summary>
        public void DeleteTrip(Trip trip)
        {
            trips.Remove(trip);
            Save();
        }

        /// <summary>
        /// Tvingar fram en sparning till JSON-filen.
        /// Denna är 'public' så att UI kan anropa den efter att ha redigerat en resa 
        /// (t.ex. ändrat budget på en befintlig resa).
        /// </summary>
        public void Save()
        {
            // Vi gör ingen try-catch här. Om det blir fel (t.ex. slut på diskutrymme),
            // låter vi felet "bubbla upp" till UI-klassen som får visa felmeddelandet för användaren.
            store.Save(trips);
        }

        // === Logik för Världskartan ===

        /// <summary>
        /// Denna metod bearbetar data för världskartan.
        /// Den filtrerar ut slutförda resor och översätter landsnamn till GeoJSON-format.
        /// </summary>
        public List<string> GetVisitedCountryNamesForMap()
        {
            // 1. Filtrera: Hämta bara resor som är markerade som "Completed"
            var completed = trips
                .Where(t => t.IsCompleted)
                .Select(t => t.Country?.Trim())         // Hämta landsnamnet och ta bort mellanslag
                .Where(c => !string.IsNullOrWhiteSpace(c)) // Ignorera tomma rader
                .Distinct(StringComparer.OrdinalIgnoreCase) // Ta bort dubbletter (Har man varit i Norge 2 ggr visas det bara en gång)
                .ToList();

            var result = new List<string>();

            // 2. Mappa: Översätt vanliga namn till officiella namn (t.ex. "USA" -> "United States of America")
            foreach (var country in completed)
            {
                var key = country!.ToLowerInvariant();

                if (CountryAlias.TryGetValue(key, out var mapped))
                    result.Add(mapped); // Använd det officiella namnet från vår lista
                else
                    result.Add(country); // Använd namnet användaren skrev in
            }
            return result;
        }

        // Dictionary för lands-alias. 
        // Static readonly eftersom listan ser likadan ut för alla användare.
        private static readonly Dictionary<string, string> CountryAlias = new()
        {
            ["usa"] = "United States of America",
            ["us"] = "United States of America",
            ["united states"] = "United States of America",
            ["uk"] = "United Kingdom",
            ["england"] = "United Kingdom",
            ["scotland"] = "United Kingdom",
            ["great britain"] = "United Kingdom",
            ["south korea"] = "Korea, Republic of",
            ["north korea"] = "Korea, Democratic People's Republic of",
            ["laos"] = "Lao People's Democratic Republic",
            ["vietnam"] = "Viet Nam",
            ["iran"] = "Iran, Islamic Republic of",
            ["bolivia"] = "Bolivia, Plurinational State of",
            ["tanzania"] = "Tanzania, United Republic of",
            ["moldova"] = "Moldova, Republic of",
            ["venezuela"] = "Venezuela, Bolivarian Republic of",
            ["syria"] = "Syrian Arab Republic"
        };
        
    }
}

