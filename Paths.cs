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
        public static readonly string BaseDir = AppContext.BaseDirectory;
        public static readonly string DataDir = Path.Combine(BaseDir, "data");
        public static readonly string UsersFile = Path.Combine(DataDir, "users.json");
    }
}
