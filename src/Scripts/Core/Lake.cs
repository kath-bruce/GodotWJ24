using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core
{
    public class Lake
    {
        private List<Hex> _hexes = new List<Hex>();
        public ReadOnlyCollection<Hex> Hexes { get => _hexes.AsReadOnly(); }
        public string Name { get; private set; }

        public Lake(List<Hex> hexes, string name)
        {
            _hexes = hexes;
            Name = name;
        }

        public bool HasHex(Hex hex)
        {
            foreach (var lakeHex in _hexes)
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