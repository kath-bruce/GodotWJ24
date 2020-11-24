using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Core
{
    public class HexMap
    {
        private List<List<Hex>> _hexes = new List<List<Hex>>();
        public ReadOnlyCollection<ReadOnlyCollection<Hex>> Hexes
        {
            get
            {
                var readonlyHexes = new List<ReadOnlyCollection<Hex>>();

                for (int i = 0; i < _hexes.Count; i++)
                {
                    readonlyHexes.Add(new ReadOnlyCollection<Hex>(_hexes[i]));
                }

                return readonlyHexes.AsReadOnly();
            }
        }

        private List<Lake> _lakes = new List<Lake>();
        public ReadOnlyCollection<Lake> Lakes { get => _lakes.AsReadOnly(); }

        private List<MountainRange> _mountainRanges = new List<MountainRange>();
        public ReadOnlyCollection<MountainRange> MountainRanges { get => _mountainRanges.AsReadOnly(); }

        private List<River> _rivers = new List<River>();
        public ReadOnlyCollection<River> Rivers { get => _rivers.AsReadOnly(); }

        public int HexMapSize { get; }
        public bool IsLocked { get; private set; } = false;

        public const int HEXES_PER_RIVER = 4;

        public HexMap(int mapSize)
        {
            HexMapSize = mapSize;
        }

        public void AddColumn(List<Hex> col)
        {
            if (!IsLocked)
            {
                _hexes.Add(col);
            }
        }

        public void Lock()
        {
            FindLakes();
            FindMountainRanges();
            CreateRivers();
            IsLocked = true;
        }

        private void CreateRivers()
        {
            //for loop each lake
            foreach (var lake in _lakes)
            {
                var noOfRivers = (int)Math.Ceiling(lake.Hexes.Count / (float)HEXES_PER_RIVER);

                for (int i = 0; i < noOfRivers; i++)
                {
                    var lakeHex = lake.Hexes[i * (HEXES_PER_RIVER - 1)];

                    var mountain = FindNearestMountain(lakeHex);

                    if (FindPathToNearestMountain(lakeHex, mountain, out List<Hex> river))
                    {
                        foreach (var riverHex in river)
                        {
                            riverHex.SetRiver();
                        }

                        _rivers.Add(new River(river, "RIVER"));
                    }
                }

                //search for closest mountain range, keeping track of current path
                //hills are impassable and if cannot find a way to a mountain then no river will be created
                //if encounter tile with a river, stop and make recorded path of hexes all have rivers on them
                //then the art can change based on what neighbours have a river
            }
        }

        private bool IsMountainReachable(Hex lakeStart, Hex mountainEnd)
        {
            var visitedHexes = new List<Hex>();
            visitedHexes.Add(lakeStart);

            var frontier = new Queue<Hex>();
            frontier.Enqueue(lakeStart);

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbour in FindHexNeighbours(current))
                {
                    if (!visitedHexes.Contains(neighbour))
                    {
                        if (neighbour.Terrain == HexTerrain.GRASSLAND)
                        {
                            frontier.Enqueue(neighbour);
                        }
                        else if (neighbour == mountainEnd)
                        {
                            return true;
                        }
                    }
                }

                visitedHexes.Add(current);
            }

            return false;
        }

        private Hex FindNearestMountain(Hex start)
        {
            var visitedHexes = new List<Hex>();
            visitedHexes.Add(start);

            var frontier = new Queue<Hex>();
            frontier.Enqueue(start);

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbour in FindHexNeighbours(current))
                {
                    if (!visitedHexes.Contains(neighbour))
                    {
                        if (neighbour.Terrain == HexTerrain.MOUNTAIN && IsMountainReachable(start, neighbour))
                        {
                            return neighbour;
                        }
                        else
                        {
                            visitedHexes.Add(neighbour);
                            frontier.Enqueue(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        private bool FindPathToNearestMountain(Hex lakeHex, Hex mountain, out List<Hex> river)
        {
            river = new List<Hex>();
            //don't add lake hex
            //rivers can only be on grassland

            if (mountain == null)
            {
                return false;
            }

            var cameFrom = new Dictionary<Hex, Hex>();

            var visitedHexes = new List<Hex>();
            visitedHexes.Add(lakeHex);

            var frontier = new Queue<Hex>();
            frontier.Enqueue(lakeHex);

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbour in FindHexNeighbours(current))
                {
                    if (!visitedHexes.Contains(neighbour))
                    {
                        if (neighbour.Terrain == HexTerrain.GRASSLAND)
                        {
                            if (!neighbour.Features.HasFlag(HexFeatures.RIVER))
                            {
                                frontier.Enqueue(neighbour);
                                frontier = new Queue<Hex>(frontier.AsQueryable().OrderBy<Hex, double>((h) =>

                                    Math.Abs(Math.Sqrt((h.Col - mountain.Col) * (h.Col - mountain.Col)
                                    + (h.Row - mountain.Row) * (h.Row - mountain.Row)))

                                ));
                                cameFrom[neighbour] = current;
                            }
                            else
                            {
                                //add to river using current path (except last in stack as that is lakehex)
                                Hex prevHex = current;

                                while (prevHex != lakeHex)
                                {
                                    river.Add(prevHex);
                                    prevHex = cameFrom[prevHex];
                                }

                                if (river.Count < 1)
                                {
                                    return false;
                                }

                                return true;
                            }
                        }
                        else if (neighbour.Terrain == HexTerrain.MOUNTAIN)// || neighbour.Terrain == HexTerrain.LAKE)
                        {
                            //add to river using current path (except last in stack as that is lakehex)

                            Hex prevHex = current;

                            while (prevHex != lakeHex)
                            {
                                river.Add(prevHex);
                                prevHex = cameFrom[prevHex];
                            }

                            if (river.Count < 1)
                            {
                                return false;
                            }

                            return true;
                        }
                    }
                }

                visitedHexes.Add(current);
            }

            return false;
        }

        private void FindMountainRanges()
        {
            for (int i = 0; i < _hexes.Count; i++)
            {
                for (int j = 0; j < _hexes[i].Count; j++)
                {
                    var hex = _hexes[i][j];

                    if (hex.Terrain == HexTerrain.MOUNTAIN && !IsHexAlreadyPartOfMountainRange(hex))
                    {
                        _mountainRanges.Add(new MountainRange(FindAllConnectedMountainHexes(hex, out string name), name));
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        private bool IsHexAlreadyPartOfMountainRange(Hex hex)
        {
            foreach (MountainRange mountainRange in _mountainRanges)
            {
                if (mountainRange.HasHex(hex))
                {
                    return true;
                }
            }

            return false;
        }

        private List<Hex> FindAllConnectedMountainHexes(Hex hex, out string name)
        {
            var mountainHexes = new List<Hex>();
            mountainHexes.Add(hex);

            var visitedHexes = new List<Hex>();
            visitedHexes.Add(hex);

            var frontier = new Queue<Hex>();
            frontier.Enqueue(hex);

            while (frontier.Count > 0)
            {
                Hex current = frontier.Dequeue();

                foreach (var neighbour in FindHexNeighbours(current))
                {
                    if (!visitedHexes.Contains(neighbour))
                    {
                        if (neighbour.Terrain == HexTerrain.MOUNTAIN)
                        {
                            mountainHexes.Add(neighbour);
                            frontier.Enqueue(neighbour);
                        }

                        visitedHexes.Add(neighbour);
                    }
                }
            }

            //after finding all connected lake hexes
            //name it
            name = "MOUNTAIN";
            return mountainHexes;
        }

        private void FindLakes()
        {
            for (int i = 0; i < _hexes.Count; i++)
            {
                for (int j = 0; j < _hexes[i].Count; j++)
                {
                    var hex = _hexes[i][j];

                    if (hex.Terrain == HexTerrain.LAKE && !IsHexAlreadyPartOfLake(hex))
                    {
                        _lakes.Add(new Lake(FindAllConnectedLakeHexes(hex, out string name), name));
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
            foreach (Lake lake in _lakes)
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
                    if (!visitedHexes.Contains(neighbour))
                    {
                        if (neighbour.Terrain == HexTerrain.LAKE)
                        {
                            lakeHexes.Add(neighbour);
                            frontier.Enqueue(neighbour);
                        }

                        visitedHexes.Add(neighbour);
                    }
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
                    neighbours.Add(_hexes[hex.Col - 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(_hexes[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(_hexes[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(_hexes[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(_hexes[hex.Col - 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(_hexes[hex.Col - 1][hex.Row]);
                }
            }
            else
            {
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthWest))
                {
                    neighbours.Add(_hexes[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(_hexes[hex.Col + 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(_hexes[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(_hexes[hex.Col + 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(_hexes[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(_hexes[hex.Col - 1][hex.Row]);
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