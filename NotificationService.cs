
using System;
using Spectre.Console;
using Travel_Journal.Models;     // för Trip
using Travel_Journal.UIServices; // för UI-klassen
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Travel_Journal.Models;
using Travel_Journal.UIServices;

namespace Travel_Journal
{
    public static class NotificationService
    {
        // 🔒 PRIVAT hjälparmetod
        // Tar emot mål, aktuellt värde och text om resan
        // Returnerar: färdigt meddelande + vilken typ av notis
        private static (string message, NotificationType type) BuildBudgetStatusMessage(
            decimal goalAmount,
            decimal currentAmount,
            string contextText)
        {
            decimal difference = goalAmount - currentAmount; // skillnaden mellan mål och nuvarande

            if (difference > 0)
            {
                // UNDER budget → Info (blå)
                string message =
                    $"You are now {difference} away from your target budget for {contextText}.";
                return (message, NotificationType.Info);
            }
            else if (difference == 0)
            {
                // EXAKT på målet → Success (grön)
                string message =
                    $"You have just reached your target budget for {contextText}.";
                return (message, NotificationType.Success);
            }
            else
            {
                // ÖVER budget → Warning (röd)
                decimal overAmount = Math.Abs(difference);
                string message =
                    $"You have exceeded your budget by {overAmount} for {contextText}.";
                return (message, NotificationType.Warning);
            }
        }

        // 📌 1) Allmän budgetnotis (popup mitt på skärmen)
        public static void ShowBudgetStatus(decimal goalAmount, decimal currentAmount, string contextText)
        {
            // Använd hjälparmetoden för att ta fram text + typ
            var (message, type) = BuildBudgetStatusMessage(goalAmount, currentAmount, contextText);

            // Visa vanlig notis via UI (den använder popup)
            UI.ShowNotification(message, type);
        }

        // 📌 2) Budgetstatus för en specifik resa – två kolumner
        public static void ShowBudgetStatusForTrip(Trip trip)
        {
            // Skapa en tydlig beskrivning av resan
            string contextText = $"Trip to {trip.City}, {trip.Country}";

            // Använd samma logik som ovan
            var (message, type) = BuildBudgetStatusMessage(
                trip.PlannedBudget,
                trip.Cost,
                contextText);

            // Här visar vi TVÅ paneler: trip-panel (vänster) + budget-panel (höger)
            // (förutsätter att du har UI.ShowPreviousTripWithBudget implementerad)
            UI.ShowPreviousTripWithBudget(trip, message, type);
        }

        // ✅ Övriga notiser (behåll, små och tydliga)

    }
}
