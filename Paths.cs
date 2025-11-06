using System;
using System.IO;

namespace Travel_Journal
{
    public static class Paths
    {
        // === Bas-katalogen där programmet körs ===
        // Exempel: bin/Debug/net8.0/
        // Används som utgångspunkt för att bygga hela filsökvägen.
        public static readonly string BaseDir = AppContext.BaseDirectory;

        // === Datamappen ===
        // Här sparas all information som JSON-filer.
        // Vi lägger både "users.json" (alla konton) och "trips.json" (per användare) här.
        public static readonly string DataDir = Path.Combine(BaseDir, "data");

        // === Användarfilen (alla registrerade konton) ===
        // Denna fil används av AccountStore för att lagra alla användare.
        public static readonly string UsersFile = Path.Combine(DataDir, "users.json");

        // === Säkerställer att datamappen finns ===
        // Anropas vid programmets start, innan vi försöker läsa eller spara filer.
        public static void EnsureDataDir()
        {
            // Om mappen "data" inte finns, skapa den automatiskt.
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);
        }
    }
}
