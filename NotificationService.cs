using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Travel_Journal
{
    public static class NotificationService
    {
        public static void ShowBudgetStatus(decimal goalAmount, decimal currentAmount, string contextText)
        // Metod som visar budget-status som en notis
        // goalAmount     = målets totalbudget
        // currentAmount  = nuvarande sparat/kostnad
        // contextText    = text för vilken resa (t.ex. "resan till Rom")
        {
            decimal difference = goalAmount - currentAmount; //räknar skillnaden mellan mål och nuvarande kostnad
            if (difference > 0) //om skillnaden är positiv-vi är under målet
            {
                string message = $"Du är nu {difference} ifrån din mål-budget för {contextText}.";//text för info notisen

                UI.ShowNotification(message, NotificationType.Info);//anropar UI för att skriva ut notisen snyggt(texten, typen)
            }
            else if (difference == 0)//om skillnaden är precis 0 då har vi nått målet
            {
                string message = $"Du har precis nått din mål-budget för contextText";//text för sucess notiden

                UI.ShowNotification(message, NotificationType.Sucess);
            }
            else //annars difference<0 då har vi gått över målet
            {
                //difference < 0 ? -difference : difference;-ett annat sätt att göra
                decimal overAmount = Math.Abs(difference);// Gör om negativt värde till positivt (hur mycket över)

                string message = $"Du har överskridit din budget med {overAmount} för {contextText}.";//text för varningen

                UI.ShowNotification(message, NotificationType.Warning);

            }

        }
        public static void ShowTripCreated(Trip trip)
        {
            string message = $"Din resa till {trip.City}, {trip.Country} har skapats.";
            UI.ShowNotification(message, NotificationType.Sucess);
        }
        public static void ShowTripCompleted(Trip trip)
        {
            string message = $"Resan till {trip.City}, {trip.Country} är nu markerad som avslutad";
            UI.ShowNotification(message, NotificationType.Info);

        }
        // public static void ShowActivityAdded(Trip trip, activityName)
        //{
        // string message = $"Aktiviteten {activityName} har lagts till på resan till {trip.Country}";
        // UI.ShowNotification(message , NotificationType.Info);
        // }

        public static void ShowBudgetStatusForTrip(Trip trip) //Metod som visar budget-status för en specifik resa
        {
            decimal goalAmount = trip.PlannedBudget;//målbudget för resan
            decimal currentAmount = trip.Cost;//nuvarande kostnad
            string contextText = $"Resan till {trip.City}, {trip.Country}";//text som beskriver resan-används i notistexten
            NotificationService.ShowBudgetStatus(goalAmount, currentAmount, contextText); //anropar vår notificationsservice med alla värden
        }
        // public void UpdateTripCost(Trip trip)
        // {
        //   Console.WriteLine("Ange ny kostnad för resan:");
        //  string input = Console.ReadLine()!;
        // decimal newCost = decimal.Parse(input);

        // trip.Cost = newCost;
        // ShowBudgetStatusForTrip(trip);// Anropar vår metod som i sin tur anropar NotificationService

       
    }
}



