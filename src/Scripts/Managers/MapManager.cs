using Godot;
using System;
using System.Collections.Generic;
using Components;
using Core;

namespace Managers
{
    public class MapManager : ManagerBase
    {
        [Export]
        private string HexComponentPath;
        [Export]
        private NodePath MapNodePath = new NodePath();
        [Export]
        public int HexMapSize { get; private set; }

        private PackedScene HexComponent;
        public Node MapNode { get; private set; }
        public List<List<Hex>> Map { get; private set; } = new List<List<Hex>>();
        public List<Lake> Lakes { get; private set; } = new List<Lake>();

        public override void InitialiseManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public override void _Ready()
        {
            MapNode = GetNode(MapNodePath);

            HexComponent = ResourceLoader.Load<PackedScene>(HexComponentPath);

            GenerateMap();

            GenerateMapSprites();

            FindLakes();
        }


        //definitely need to move this into a separate hexmap class

        private void FindLakes()
        {
            //probs could just put this in generate map but will have this here just now
            //for loop through each hex
            //if not lake and isn't already part of a lake
            //continue
            //if lake
            //create new list of hexes
            //add current hex to list of hexes
            //use that hex to start a breadth first search of other connected lake hexes

            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    var hex = Map[i][j];

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

            //ChangeLakeNames();
            //GD.Print($"no of lakes found: {Lakes.Count}");

        }

        private void ChangeLakeNames()
        {
            for(int i = 0; i < Lakes.Count; i++)
            {
                foreach (var hex in Lakes[i].Hexes)
                {
                    var hexComponent = GetHexComponent(hex);
                    hexComponent.SetNameOfFeature($"Lake {i}");
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
                    neighbours.Add(Map[hex.Col - 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(Map[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(Map[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(Map[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(Map[hex.Col - 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(Map[hex.Col - 1][hex.Row]);
                }
            }
            else
            {
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthWest))
                {
                    neighbours.Add(Map[hex.Col][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
                {
                    neighbours.Add(Map[hex.Col + 1][hex.Row - 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.East))
                {
                    neighbours.Add(Map[hex.Col + 1][hex.Row]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
                {
                    neighbours.Add(Map[hex.Col + 1][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
                {
                    neighbours.Add(Map[hex.Col][hex.Row + 1]);
                }
                if (hex.Neighbours.HasFlag(HexNeighbours.West))
                {
                    neighbours.Add(Map[hex.Col - 1][hex.Row]);
                }
            }

            return neighbours;
        }

        public Vector2 GetBottomRightCornerOfMapInPixelCoords()
        {
            HexComponent bottomRight = MapNode.GetChild<HexComponent>(MapNode.GetChildCount() - 1);

            Sprite sprite = bottomRight.GetNode<Sprite>(new NodePath("HexSprite"));

            var size = sprite.Texture.GetSize(); //width and height in pixels

            return new Vector2(bottomRight.Position.x + (size.x / 2f), bottomRight.Position.y + (size.y / 2f));
        }

        private void GenerateMap()
        {
            var noise = new OpenSimplexNoise();
            noise.Lacunarity = 450f;
            noise.Period = 2f;
            var rnd = new RandomNumberGenerator();
            rnd.Randomize();
            noise.Seed = (int)rnd.Randi();
            var image = noise.GetImage(HexMapSize, HexMapSize);
            image.Lock();

            for (int i = 0; i < HexMapSize; i++)
            {
                List<Hex> newCol = new List<Hex>();

                for (int j = 0; j < HexMapSize; j++)
                {
                    var noiseSample = image.GetPixel(i, j).r; //noise is grayscale so only need r

                    HexTerrain hexTerr = HexTerrain.NULL;

                    if (noiseSample >= 0.7f)
                    {
                        //mountains
                        hexTerr = HexTerrain.MOUNTAIN;
                    }
                    else if (noiseSample >= 0.5f)
                    {
                        //hills
                        hexTerr = HexTerrain.HILLS;
                    }
                    else if (noiseSample >= 0.3f)
                    {
                        //grassland
                        hexTerr = HexTerrain.GRASSLAND;
                    }
                    else
                    {
                        //lake
                        hexTerr = HexTerrain.LAKE;
                    }

                    newCol.Add(new Hex(i, j, hexTerr, HexFeatures.NONE, FindValidNeighbours(i, j)));
                }

                Map.Add(newCol);
            }
        }

        //the hexmap class could handle this
        private HexNeighbours FindValidNeighbours(int x, int y)
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

        private void GenerateMapSprites()
        {
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    var newHex = HexComponent.Instance() as HexComponent;
                    newHex.Hex = Map[i][j];

                    //don't like magic strings!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    var sprite = newHex.GetNode<Sprite>(new NodePath("HexSprite"));
                    sprite.Modulate = GetHexColour(newHex.Hex.Terrain);

                    //change hex position
                    var spriteSize = sprite.Texture.GetSize();
                    var pos = spriteSize * new Vector2(i, j);
                    pos.y *= 0.75f;
                    if ((j % 2) == 1)
                    {
                        pos.x += (spriteSize.x) / 2f;
                    }
                    newHex.Position = pos + (spriteSize / 2f);
                    newHex.Name = $"Hex({i},{j})";
                    //newHex.SetNameOfFeature(newHex.Name);
                    //newHex.EnableNeighbourIndicators();

                    MapNode.AddChild(newHex, true);
                }
            }
        }

        private HexComponent GetHexComponent(Hex hex)
        {
            return MapNode.GetNode<HexComponent>(new NodePath($"Hex({hex.Col},{hex.Row})"));
        }

        //there's probably a better of doing this but that's a problem for Tomorrow's Kathleen
        private Color GetHexColour(HexTerrain terr)
        {
            switch (terr)
            {
                case HexTerrain.MOUNTAIN:
                    return Colors.AntiqueWhite;

                case HexTerrain.HILLS:
                    return Colors.YellowGreen;

                case HexTerrain.BOG:
                    return Colors.SaddleBrown;

                case HexTerrain.GRASSLAND:
                    return Colors.LawnGreen;

                case HexTerrain.RIVER:
                    return Colors.AliceBlue;

                case HexTerrain.LAKE:
                    return Colors.DeepSkyBlue;

                default:
                    return Colors.Magenta;
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }

}
