using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public class TripDraft // Ett enkelt "utkast-objekt" som håller allt som kan ändras
                          // i en upcoming trip när vi jobbar med Undo/Redo.
    {
        public string Country { get; set; }
        public string City { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Konstruktor som fyller alla fält.
        public TripDraft(string country, string city, decimal budget, DateTime startDate, DateTime endDate)
        {
            Country = country;
            City = city;
            Budget = budget;
            StartDate = startDate;
            EndDate = endDate;
        }
        




    }
}
