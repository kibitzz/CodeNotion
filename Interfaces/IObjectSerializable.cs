using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basicClasses.Interfaces
{
    interface IObjectSerializable
    {
        string Serialize();
        void Deserialize(string data);
    }
}
