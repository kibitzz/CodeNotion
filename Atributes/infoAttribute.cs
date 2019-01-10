using System;

namespace basicClasses.models
{
    public class infoAttribute : Attribute
    {
        public string v;

        public infoAttribute(string v)
        {
            this.v = v;
        }
    }
}