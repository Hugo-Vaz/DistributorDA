using System;

namespace DistDataAquisition
{
    internal class Identity : Attribute
    {
        public string Name { get; set; }
    }

    internal class AnotherObject : Attribute
    {
        public string Name { get; set; } 
    }

    internal class TrackChangesOnLoging : Attribute
    {
        public string Name { get; set; } 
    }
}