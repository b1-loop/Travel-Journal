using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_Journal
{
    public enum NotificationType  //enum som visa vilken typ av notis ska vi ha
    {
        Info,   //t.ex du är X ifrån målet
        Success,  //t.ex du har nått din budget
        Warning  //t.ex du har överskridit budgeten
    }
}
