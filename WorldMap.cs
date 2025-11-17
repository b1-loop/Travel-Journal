using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;

using Spectre.Console;


// Alias så vi vet att vi menar ImageSharps Color
using ImgColor = SixLabors.ImageSharp.Color;

namespace Travel_Journal
{
    internal static class WorldMap
    {
        // Länder -> pixel-koordinater på din world-bild (1920 x 1229)
        // X = vänster→höger, Y = upp→ner (innan offset)
        private static readonly Dictionary<string, (float x, float y)> CountryPixels =
            new(StringComparer.OrdinalIgnoreCase)
            {
                // AFRICA
                ["Algeria"] = (971, 423),
                ["Egypt"] = (1120, 437),
                ["Ethiopia"] = (1173, 560),
                ["Kenya"] = (1163, 614),
                ["Libya"] = (1051, 430),
                ["Morocco"] = (928, 396),
                ["Namibia"] = (1051, 765),
                ["Nigeria"] = (1003, 553),
                ["South Africa"] = (1093, 819),
                ["Sudan"] = (1120, 512),
                ["Tanzania"] = (1147, 655),
                ["Tunisia"] = (1008, 382),
                ["Zimbabwe"] = (1115, 744),
                ["Botswana"] = (1088, 765),

                // EUROPE
                ["Austria"] = (1035, 294),
                ["Belgium"] = (981, 273),
                ["Bulgaria"] = (1093, 328),
                ["Croatia"] = (1045, 307),
                ["Czech Republic"] = (1040, 280),
                ["Denmark"] = (1013, 232),
                ["Finland"] = (1099, 178),
                ["France"] = (971, 300),
                ["Germany"] = (1013, 266),
                ["Greece"] = (1077, 348),
                ["Hungary"] = (1061, 294),
                ["Iceland"] = (864, 171),
                ["Ireland"] = (917, 253),
                ["Italy"] = (1024, 328),
                ["Netherlands"] = (987, 259),
                ["Norway"] = (1013, 191),
                ["Poland"] = (1061, 259),
                ["Portugal"] = (917, 348),
                ["Romania"] = (1093, 300),
                ["Slovakia"] = (1061, 287),
                ["Slovenia"] = (1040, 300),
                ["Spain"] = (939, 341),
                ["Sweden"] = (1056, 205),
                ["Switzerland"] = (1003, 294),
                ["UK"] = (949, 246),
                ["United Kingdom"] = (949, 246),

                // MIDDLE EAST / WEST ASIA
                ["Israel"] = (1147, 396),
                ["Qatar"] = (1232, 444),
                ["Saudi Arabia"] = (1200, 451),
                ["Turkey"] = (1147, 348),
                ["UAE"] = (1248, 451),
                ["United Arab Emirates"] = (1248, 451),

                // ASIA
                ["Bangladesh"] = (1440, 451),
                ["China"] = (1520, 376),
                ["India"] = (1376, 464),
                ["Indonesia"] = (1589, 628),
                ["Japan"] = (1696, 369),
                ["Kazakhstan"] = (1323, 287),
                ["Malaysia"] = (1504, 587),
                ["Nepal"] = (1408, 423),
                ["Pakistan"] = (1333, 410),
                ["Philippines"] = (1611, 526),
                ["Russia"] = (1440, 205),
                ["Singapore"] = (1515, 608),
                ["South Korea"] = (1643, 369),
                ["Thailand"] = (1499, 512),
                ["Vietnam"] = (1536, 505),

                // NORTH AMERICA
                ["Canada"] = (448, 232),
                ["Mexico"] = (416, 457),
                ["USA"] = (437, 348),
                ["United States"] = (437, 348),
                ["Guatemala"] = (480, 512),
                ["Honduras"] = (501, 512),
                ["Panama"] = (533, 553),
                ["Cuba"] = (539, 471),
                ["Dominican Republic"] = (587, 485),

                // SOUTH AMERICA
                ["Argentina"] = (619, 874),
                ["Bolivia"] = (619, 724),
                ["Brazil"] = (683, 683),
                ["Chile"] = (581, 819),
                ["Colombia"] = (565, 587),
                ["Ecuador"] = (544, 621),
                ["Peru"] = (560, 676),
                ["Venezuela"] = (608, 567),

                // EXTRA
                ["Australia"] = (1680, 785),
                ["New Zealand"] = (1888, 901),
            };

        /// <summary>
        /// Visar världskartan med en pin per land där användaren har minst en resa.
        /// </summary>
        public static void ShowWorldMap(IEnumerable<Trip> trips)
        {
            var tripList = trips.ToList();

            if (!tripList.Any())
            {
                AnsiConsole.MarkupLine("[grey]No trips yet. Add a trip to see pins on the world map.[/]");
                return;
            }

            // ändra sökvägen om du byter namn på bilden
            const string worldImagePath = "Images/image.png.jpg";
            if (!File.Exists(worldImagePath))
            {
                AnsiConsole.MarkupLine($"[red]World map image not found at: {worldImagePath}[/]");
                return;
            }

            using var image = Image.Load<Rgba32>(worldImagePath);

            // vilka länder har minst en resa?
            var visitedCountries = tripList
                .Select(t => t.Country?.Trim())
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (visitedCountries.Count == 0)
            {
                AnsiConsole.MarkupLine("[grey]Trips found, but no countries set.[/]");
                return;
            }

            // Offset i Y-led för att kompensera för "MAP OF THE WORLD"-bannern + extra hav
            const int MapOffsetY = 150;

            // Rita en tydlig pin för varje land
            foreach (var country in visitedCountries)
            {
                if (!CountryPixels.TryGetValue(country!, out var pos))
                    continue; // landet saknar koordinater i kartan

                var (x, y) = pos;

                // flytta ner lite
                y += MapOffsetY;

                // neon-röd fyllning + gul fet kant
                var fillColor = ImgColor.FromRgb(255, 20, 20);    // stark röd
                var borderColor = ImgColor.FromRgb(255, 255, 0);    // gul

                const float radius = 45f; // stor pin

                image.Mutate(ctx =>
                {
                    var circle = new EllipsePolygon(x, y, radius);

                    var brush = Brushes.Solid(fillColor);
                    var pen = Pens.Solid(borderColor, 12f); // tjock kant

                    ctx.Fill(brush, circle);
                    ctx.Draw(pen, circle);
                });
            }

            // Spara temp-bild och visa i terminalen
            const string outputPath = "world_rendered.png";
            image.Save(outputPath);

            var canvas = new CanvasImage(outputPath);
            canvas.MaxWidth(Console.WindowWidth - 4);
            AnsiConsole.Write(canvas);

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red]●[/] [white]Countries with at least one trip[/]");
        }
    }
}