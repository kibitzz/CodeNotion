using System;

namespace basicClasses.models
{
    internal class appliableAttribute : Attribute
    {
        public string v;

        public appliableAttribute(string v)
        {
            this.v = v;
        }
    }
}