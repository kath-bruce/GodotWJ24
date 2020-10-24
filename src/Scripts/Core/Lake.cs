using System.Collections.Generic;

namespace Core
{
    public class Lake
    {
        public List<Hex> Hexes { get; private set; }
        public string Name { get; private set; }

        public Lake(List<Hex> hexes, string name)
        {
            Hexes = hexes;
            Name = name;
        }

        public bool HasHex(Hex hex)
        {
            foreach (var lakeHex in Hexes)
            {
                if (lakeHex.Col == hex.Col && lakeHex.Row == hex.Row)
                {
                    return true;
                }
            }

            return false;
        }
    }
}