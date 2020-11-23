using System.Collections.Generic;

namespace Core
{
    public class River : MultiHexFeature
    {
        public River(List<Hex> hexes, string name)
        : base(hexes, name) {}
    }
}