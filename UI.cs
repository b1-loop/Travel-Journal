using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public static class UI
    {
        public static void Splash()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Travel Journal").Centered().Color(Color.MediumPurple));
            AnsiConsole.Write(new Rule("[grey] Team 1 — Code Commanders[/]").LeftJustified());
            TypeOut("Välkommen! Navigera via menyn nedan.");

        }


        public static string MainMenu()
        {
            return AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .Title("[bold cyan]Välj:[/]")
            .HighlightStyle(new Style(Color.DeepSkyBlue1))
            .AddChoices("Register", "Login", "Forgot password", "Exit"));
        }


        public static void Transition(string title)
        {
            AnsiConsole.Write(new Rule($"[white]{title}[/]").RuleStyle("grey50"));
        }


        public static void WithStatus(string text, Action action)
        {
            AnsiConsole.Status().Spinner(Spinner.Known.Dots2).Start(text, _ =>
            {
                action();
                System.Threading.Thread.Sleep(300);
            });
        }


        //public static void ShowPath()
        //{
        //    var p = new Panel($"[grey]Datamapp:[/] {Paths.DataDir}\n[grey]Users JSON:[/] {Paths.UsersFile}")
        //    {
        //        Border = BoxBorder.Rounded,
        //        BorderStyle = new Style(Color.Grey50),
        //        Header = new PanelHeader("Sökvägar", Justify.Center)
        //    };
        //    AnsiConsole.Write(p);
        //}


        public static void Success(string msg)
        {
            var panel = new Panel($"[green]✅ {msg}[/]") { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Green) };
            AnsiConsole.Write(panel);
        }
        public static void Error(string msg)
        {
            var panel = new Panel($"[red]❌ {msg}[/]") { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Red) };
            AnsiConsole.Write(panel);
        }
        public static void Warn(string msg)
        {
            var panel = new Panel($"[yellow]⚠ {msg}[/]") { Border = BoxBorder.Rounded, BorderStyle = new Style(Color.Yellow) };
            AnsiConsole.Write(panel);
        }


        public static void TypeOut(string text)
        {
            foreach (var ch in text)
            {
                AnsiConsole.Markup($"[grey]{Markup.Escape(ch.ToString())}[/]");
                System.Threading.Thread.Sleep(6);
            }
            AnsiConsole.WriteLine();
        }
    }
}

