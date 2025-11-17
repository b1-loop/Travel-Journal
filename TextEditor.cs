using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public class TextEditor//en text editor verktyg som använder UndoRedoManager<string> och testar Undo/Redo med text
    {
        private readonly UndoRedoManager<string> _undo;//UndoRedo manager som hanterar string,
                                                       //_undo används överallt i klassen för att göra Undo/Redo
 
        public TextEditor(string initialText) //skapar konstruktor som körs direkt när man skriver ny text
                                              //initialText-texten som editorn ska börja med(kan det vara tom text med?)
        {
            _undo = new UndoRedoManager<string>(initialText); //// Skapar en ny UndoRedoManager för string och
                                                              //skickar in initialText som första state.
        }
        public void RunMenu()//skapar metod som startar och kör text-editorn i konsolmeny
        {
            bool isRunning = true; // Flagga som styr om menyn ska fortsätta köras eller avslutas.

            while (isRunning)// Meny-loop – kör så länge isRunning är true.
            {
                Console.Clear();//rensa konsolen så att innehållet blir snyggt

                //Skriver ut Titel för editorn
                Console.WriteLine("Text Editor med Undo/Redo");
                Console.WriteLine();

                //Visa nuvarande text från UndoRedoManager
                Console.WriteLine("Aktuell Text:");
                Console.WriteLine(_undo.CurrentState);
                Console.WriteLine();

                //Skriver ut menyval
                Console.WriteLine("1.Skriv ny text:");
                Console.WriteLine("2.Undo(ångra)");
                Console.WriteLine("3.Redo(gör om)");
                Console.WriteLine("0.Avsluta");
                Console.WriteLine("Välj ett ålternativ(1-0): ");

                string choice = Console.ReadLine();//läs in användarens val(den nya texten)

                switch (choice)//hantera de olika val
                {
                    case "1"://Alternativ 1: Ny text skrivs
                        Console.WriteLine("Skriv ny text");
                        string newText = Console.ReadLine()!;
                        _undo.ApplyChange(newText);//ny ändring
                        break;

                    case "2":// Kontrollera om Undo är möjligt.
                        if (_undo.IsUndo)
                        {


                            _undo.Undo();// Utför Undo.
                            Console.WriteLine("Ångrade senaste ändringen");// Meddela användaren.
                        }
                        else
                        {
                            Console.WriteLine($"Finns inget att ångra");// Om inget Undo finns, visa meddelande
                        }
                        Pause();
                        break;

                    case "3":// Alternativ 3: Redo
                        if (_undo.IsRedo)// Kontrollera om Redo är möjligt.
                        {
                            _undo.Redo();// Utför Redo.
                            Console.WriteLine("Gjorde om senaste ångringen");// Meddela användaren
                        }
                        else
                        {
                            Console.WriteLine("Finns inget att göra om");// Om inget Redo finns.
                        }
                        Pause();
                        break;

                    case "0":  // Avsluta loopen genom att sätta flaggan till false.
                        isRunning = false;
                        break;

                        //Om användaren skriver något annat
                    default:
                        // Visa att valet är ogiltigt.
                        Console.WriteLine("Ogiltigt val, försök igen.");

                        // Vänta på tangenttryck innan menyn visas igen.
                        Pause();
                        break;

                }


            }
        }
        // Ny statisk metod som används i TripService
        public static string EditTextWithUndo(string initialText)
        {
            var undo = new UndoRedoManager<string>(initialText);
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== Edit Text with Undo/Redo ===");
                Console.WriteLine();
                Console.WriteLine("Aktuell text:");
                Console.WriteLine(undo.CurrentState);
                Console.WriteLine();

                Console.WriteLine("1. Skriv ny text");
                Console.WriteLine("2. Undo (ångra)");
                Console.WriteLine("3. Redo (gör om)");
                Console.WriteLine("0. Spara och gå tillbaka");
                Console.Write("Val: ");

                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        Console.Write("Ny text: ");
                        string newText = Console.ReadLine()!;
                        undo.ApplyChange(newText);
                        break;

                    case "2":
                        if (undo.IsUndo)
                        {
                            undo.Undo();
                        }
                        else
                        {
                            Console.WriteLine("Det finns inget att ångra.");
                        }
                        break;

                    case "3":
                        if (undo.IsRedo)
                        {
                            undo.Redo();
                        }
                        else
                        {
                            Console.WriteLine("Det finns inget att göra om.");
                        }
                        break;

                    case "0":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            }

            return undo.CurrentState;
        }
        
      
       
        private void Pause()// Hjälpmetod som pausar konsolen tills användaren trycker på en tangent.
        {
            Console.WriteLine();
            Console.WriteLine("Tryck på valfri tagent för att fortsätta...");// Instruktion till användaren.
            Console.ReadKey(true);// Väntar på ett knapptryck.
        }
    }
}
        


    

