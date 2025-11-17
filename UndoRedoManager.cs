using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public class UndoRedoManager<T>
    {
        private readonly Stack<T> _undoStack = new();//stack som sparar historiken bakåt
        private readonly Stack<T> _redoStack = new();//stack som sparar historiken framåt
        public T CurrentState { get; private set; } //det nuvarande läget-text

        //Skapar konstruktor och skickar in ett start-state
        public UndoRedoManager(T initialState)
        {
            CurrentState = initialState; //Aktuellt state sätts till startvärdet
        }

        //Kollar om Undo är möjligt
        public bool IsUndo => _undoStack.Count > 0;

        //Kollar om Redo är möjligt
        public bool IsRedo => _redoStack.Count > 0;

        public void ApplyChange(T newState)   // finns redan
        {
            _undoStack.Push(CurrentState);    // lägg nuvarande state i undo-stack
            CurrentState = newState;          // uppdatera aktuell state
            _redoStack.Clear();               // rensa redo-historik
        }

        public bool Undo()
        {
            if (_undoStack.Count == 0)
                return false;

            _redoStack.Push(CurrentState);
            CurrentState = _undoStack.Pop();
            return true;
        }

        public bool Redo()
        {
            if (_redoStack.Count == 0)
                return false;

            _undoStack.Push(CurrentState);
            CurrentState = _redoStack.Pop();
            return true;
        }
        

        //  NY METOD
       // public void SetState(T newState)
       // {
       //     ApplyChange(newState);            // återanvänd samma logik
       // }

       // public void ClearHistory()//skapar metod för att tömma historiken(om man vill börja om)
       // {
        //    _undoStack.Clear();
        //    _redoStack.Clear();
        //}
    }
}

