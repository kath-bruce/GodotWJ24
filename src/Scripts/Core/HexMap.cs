using System;
using System.Collections.Generic;

namespace Core
{
    public class HexMap
    {
        public List<List<Hex>> Hexes { get; private set; } = new List<List<Hex>>();
        public List<Lake> Lakes { get; private set; } = new List<Lake>();

        public int HexMapSize { get; }
        public bool IsLocked { get; private set; } = false;

        public HexMap(int mapSize)
        {
            HexMapSize = mapSize;
        }

        public void AddColumn(List<Hex> col)
        {
            if (!IsLocked)
            {
                Hexes.Add(col);
            }
        }

        public void Lock()
        {
            IsLocked = true;
            FindLakes();
        }

        private void FindLakes()
        {
            for (int i = 0; i < Hexes.Count; i++)
            {
                for (int j = 0; j < Hexes[i].Count; j++)
                {
                    var hex = Hexes[i][j];

                    if (hex.Terrain == HexTerrain.LAKE && !IsHexAlreadyPartOfLake(hex))
                    {
                        Lakes.Add(new Lake(FindAllConnectedLakeHexes(hex, out string name), name));

                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private bool IsHexAlreadyPartOfLake(Hex hex)
        {
            foreach (Lake lake in Lakes)
            {
                if (lake.HasHex(hex))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Hex> FindAllConnectedLakeHexes(Hex hex, out string name)
        {
            var lakeHexes = new List<Hex>();
            lakeHexes.Add(hex);

            var visitedHexes = new List<Hex>();
            visitedHexes.Add(hex);

            var frontier = new Queue<Hex>();
            frontier.Enqueue(hex);

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbour in FindHexNeighbours(current))
                {
                    if (!visitedHexes.Contains(neighbour) &&
                     neighbour.Terrain == HexTerrain.LAKE)
                    {
                        lakeHexes.Add(neighbour);
                        frontier.Enqueue(neighbour);
                    }

                    visitedHexes.Add(neighbour);
                }
            }

            //after finding all connected lake hexes
            //name it
            name = "LAKE";
            return lakeHexes;
        }

        private List<Hex> FindHexNeighbours(Hex hex)
        {
            //x = col
            //y = row

            var neighbours = new List<Hex>();

            if (hex.Row % 2 == 0)
            {
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthWest))
                {
                    neighbours.Add(Hexes[hex.Col - 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(Hexes[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(Hexes[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(Hexes[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(Hexes[hex.Col - 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(Hexes[hex.Col - 1][hex.Row]);
                }
            }
            else
            {
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthWest))
                {
                    neighbours.Add(Hexes[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(Hexes[hex.Col + 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(Hexes[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(Hexes[hex.Col + 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(Hexes[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(Hexes[hex.Col - 1][hex.Row]);
                }
            }

            return neighbours;
        }

        public HexNeighbours FindValidNeighbours(int x, int y)
        {
            HexNeighbours validNeighbours = HexNeighbours.None;

            if (y % 2 == 0)
            {
                if (x > 0)
                {
                    validNeighbours |= HexNeighbours.West;

                    if (y > 0)
                    {
                        validNeighbours |= HexNeighbours.NorthWest;
                    }

                    if (y < HexMapSize - 1)
                    {
                        validNeighbours |= HexNeighbours.SouthWest;
                    }
                }

                if (y > 0)
                {
                    validNeighbours |= HexNeighbours.NorthEast;
                }

                if (y < HexMapSize - 1)
                {
                    validNeighbours |= HexNeighbours.SouthEast;
                }

                if (x < HexMapSize - 1)
                {
                    validNeighbours |= HexNeighbours.East;
                }
            }
            else
            {
                if (x > 0)
                {
                    validNeighbours |= HexNeighbours.West;
                }

                if (x < HexMapSize - 1)
                {
                    validNeighbours |= HexNeighbours.East;
                }

                if (y > 0)
                {
                    validNeighbours |= HexNeighbours.NorthWest;

                    if (x < HexMapSize - 1)
                    {
                        validNeighbours |= HexNeighbours.NorthEast;
                    }
                }

                if (y < HexMapSize - 1)
                {
                    validNeighbours |= HexNeighbours.SouthWest;

                    if (x < HexMapSize - 1)
                    {
                        validNeighbours |= HexNeighbours.SouthEast;
                    }
                }
            }

            return validNeighbours;
        }
    }
}