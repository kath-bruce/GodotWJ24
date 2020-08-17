using System;

namespace Core
{
    public class Hex
    {
        public int Col { get; private set; }
        public int Row { get; private set; }

        public Hex(int col, int row)
        {
            Col = col;
            Row = row;
        }
    }
}
