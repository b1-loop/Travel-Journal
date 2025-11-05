using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public static class AccountStore
    {
        private static List<Account> accounts = new();


        public static void Load()
        {
            Directory.CreateDirectory(Paths.DataDir);
            if (!File.Exists(Paths.UsersFile)) return;
            var json = File.ReadAllText(Paths.UsersFile);
            accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new();
        }


        public static void LoadWithProgress()
        {
            Directory.CreateDirectory(Paths.DataDir);
            AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
            new TaskDescriptionColumn(),
            new ProgressBarColumn(),
            new PercentageColumn(),
            new SpinnerColumn(Spinner.Known.Dots)
                        })
            .Start(ctx =>
            {
                var t = ctx.AddTask("Laddar användare", maxValue: 100);
                for (int i = 0; i <= 100; i += 25)
                {
                    t.Value = i;
                    System.Threading.Thread.Sleep(60);
                }
                if (File.Exists(Paths.UsersFile))
                {
                    var json = File.ReadAllText(Paths.UsersFile);
                    accounts = JsonSerializer.Deserialize<List<Account>>(json) ?? new();
                }
            });
        }


        public static void Save()
        {
            Directory.CreateDirectory(Paths.DataDir);
            var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Paths.UsersFile, json);
        }


        public static bool Exists(string username) => accounts.Any(a => a.UserName == username);
        public static void Add(Account acc) => accounts.Add(acc);
        public static Account? Get(string username) => accounts.FirstOrDefault(a => a.UserName == username);
        public static void Update(Account acc)
        {
            var idx = accounts.FindIndex(a => a.UserName == acc.UserName);
            if (idx >= 0) accounts[idx] = acc;
        }
    }
}




