using System;

namespace Core
{
    public enum HexTerrain { NULL = -1, MOUNTAIN, HILLS, GRASSLAND, LAKE }

    [Flags]
    public enum HexFeatures
    {
        NONE = 0,
        FOREST = 1,
        RIVER = 2,
        BOG = 4,
        VILLAGE = 8,
        CAMP = 16,
        CAVE = 32,
        SPECIAL = 64
    }

    [Flags]
    public enum HexNeighbours
    {
        None = 0,
        NorthWest = 1,
        NorthEast = 2,
        East = 4,
        SouthEast = 8,
        SouthWest = 16,
        West = 32
    }

    public class Hex
    {
        public HexMap HexMap { get; private set; }

        public int Col { get; private set; } //x
        public int Row { get; private set; } //y
        public int Cost { get; private set; } //cost to move from hex

        public HexTerrain Terrain { get; private set; }
        public HexFeatures Features { get; private set; }
        public HexNeighbours Neighbours { get; private set; }
        public MultiHexFeature ParentMultiHexFeature { get; private set; }

        public Hex(HexMap map, int col, int row, int cost, HexTerrain terr, HexFeatures feat)
        {
            HexMap = map;
            Col = col;
            Row = row;
            Terrain = terr;
            Features = feat;
            Neighbours = HexMap.FindValidNeighbours(col, row);
        }

        public void SetRiver()
        {
            Features |= HexFeatures.RIVER;
        }

        public void SetParentMultiHexFeature(MultiHexFeature feature)
        {
            ParentMultiHexFeature = feature;
        }
    }
}
