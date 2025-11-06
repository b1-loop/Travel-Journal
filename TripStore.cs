using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Travel_Journal
{
    public static class TripStore
    {
        private static List<Trip> _trips = new();

        // Hämtar alla resor 
        public static IReadOnlyList<Trip> All => _trips;

        // Laddar alla resor från trips.json
        public static void Load()
        {
            Directory.CreateDirectory(Paths.DataDir);
            if (!File.Exists(Paths.TripsFile)) return;

            var json = File.ReadAllText(Paths.TripsFile);
            _trips = JsonSerializer.Deserialize<List<Trip>>(json) ?? new();
        }
        // Lägger till ny resa och sparar direkt
        public static void Add(Trip trip)
        {
            _trips.Add(trip);
            Save();
        }

        // Sparar alla resor till trips.json
        public static void Save()
        {
            var json = JsonSerializer.Serialize(_trips, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Paths.TripsFile, json);

            //  Visa JSON i konsolen för testning
            Console.WriteLine("JSON data saved to trips.json:");
            Console.WriteLine(json);
        }
        // Enkel modell för en resa
        public class Trip
        {
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string UserName { get; set; } = string.Empty;
            public string Type { get; set; } = "upcoming"; // eller "previous"
            public string Title { get; set; } = string.Empty;
            public string Location { get; set; } = string.Empty;
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Notes { get; set; } = string.Empty;
        }
    }

}

