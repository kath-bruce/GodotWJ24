using System.Collections.Generic;

namespace Core
{
    public class Lake : MultiHexFeature
    {
        public Lake(List<Hex> hexes, string name)
        : base(hexes, name) {}
    }
}