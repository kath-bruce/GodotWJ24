using System;

namespace Core
{
    public enum HexTerrain { NULL = -1, MOUNTAIN, HILLS, GRASSLAND, BOG, RIVER, LAKE}
    public enum HexFeatures { NONE = 0, FOREST, VILLAGE, CAMP, CAVE, SPECIAL }

    public class Hex
    {
        public int Col { get; private set; }
        public int Row { get; private set; }

        public HexTerrain Terrain { get; private set; }
        public HexFeatures Features { get; private set; }

        public Hex(int col, int row, HexTerrain terr, HexFeatures feat)
        {
            Col = col;
            Row = row;
            Terrain = terr;
            Features = feat;
        }
    }
}
