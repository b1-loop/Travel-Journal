using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Spectre.Console;

namespace Travel_Journal
{
    public static class Paths
    {
        // Bas-katalogen där programmet körs (t.ex. bin/Debug/net8.0/)
        public static readonly string BaseDir = AppContext.BaseDirectory;

        // Katalogen där datafiler sparas (skapas som en undermapp till BaseDir)
        public static readonly string DataDir = Path.Combine(BaseDir, "Data-JSON");

        // Den fullständiga sökvägen till användarfilen (users.json) i Data-JSON-mappen
        public static readonly string UsersFile = Path.Combine(DataDir, "users.json");

        // Den fullständiga sökvägen till resor (trips.json) i samma mapp
        public static readonly string TripsFile = Path.Combine(DataDir, "trips.json");
    }

}
