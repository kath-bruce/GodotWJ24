using System;

namespace Core
{
    public enum HexTerrain { NULL = -1, MOUNTAIN, HILLS, GRASSLAND, BOG, RIVER, LAKE }
    public enum HexFeatures { NONE = 0, FOREST, VILLAGE, CAMP, CAVE, SPECIAL }

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
        public int Col { get; private set; } //x
        public int Row { get; private set; } //y

        public HexTerrain Terrain { get; private set; }
        public HexFeatures Features { get; private set; }
        public HexNeighbours Neighbours { get; private set; }

        public Hex(int col, int row, HexTerrain terr, HexFeatures feat, HexNeighbours neighbours)
        {
            Col = col;
            Row = row;
            Terrain = terr;
            Features = feat;
            Neighbours = neighbours;
        }

    }
}
