using System;

namespace basicClasses.models
{
    public class modelAttribute : Attribute
    {
        public string v;

        public modelAttribute(string v)
        {
            this.v = v;
        }
    }
}