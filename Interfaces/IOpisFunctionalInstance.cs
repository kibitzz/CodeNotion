using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace basicClasses
{
    interface IOpisFunctionalInstance
    {
        string name { get; }

        opis Process(string internl, opis environment);

        void ProcessWaiter(opis evento, opis sender);

        //void bind(opis context);
    }

    interface IOpisFuncInstanceWaiter
    {
        void ProcessWaiter(opis evento, opis sender);
    }


    }
