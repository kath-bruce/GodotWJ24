using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core
{
    public class MultiHexFeature
    {
        private List<Hex> _hexes = new List<Hex>();
        public ReadOnlyCollection<Hex> Hexes { get => _hexes.AsReadOnly(); }
        public string Name { get; private set; }

        public MultiHexFeature(List<Hex> hexes, string name)
        {
            _hexes = hexes;
            Name = name;
        }

        public bool HasHex(Hex hex)
        {
            foreach (var currHex in _hexes)
            {
                if (currHex.Col == hex.Col && currHex.Row == hex.Row)
                {
                    return true;
                }
            }

            return false;
        }
    } 
}