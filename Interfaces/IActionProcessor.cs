using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses
{ 
    interface IActionProcessor
    {
        string name { get; }
        opis DataModel { get; }
        opis ActionModel { get; }

        opis log { get; }

        void InitAct(SysInstance ins);

        void CpecifyActionModel(opis specification);

        opis GetModelSpecTemplate();

        opis GetMessageModel();

        void Process(opis message);
      
    }
}
