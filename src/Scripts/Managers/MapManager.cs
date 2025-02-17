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
        public Node MapNode { get; private set; }
        public HexMap HexMap { get; private set; }
        private PackedScene HexComponent;
        private NodePath HexSpritePath;
        private Dictionary<Hex, HexComponent> HexComponentDict = new Dictionary<Hex, HexComponent>();
        
        public override void InitialiseManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public override void _Ready()
        {
            MapNode = GetNode(MapNodePath);

            HexComponent = ResourceLoader.Load<PackedScene>(HexComponentPath);

            GenerateMap();
        }

        public Vector2 GetBottomRightCornerOfMapInPixelCoords()
        {
            HexComponent bottomRight = MapNode.GetChild<HexComponent>(MapNode.GetChildCount() - 1);

            Sprite sprite = bottomRight.GetNode<Sprite>(bottomRight.SpritePath);

            var size = sprite.Texture.GetSize(); //width and height in pixels

            return new Vector2(bottomRight.Position.x + (size.x / 2f), bottomRight.Position.y + (size.y / 2f));
        }

        private void GenerateMap()
        {
            //possibly overlay this with another noise map

            HexMap = new HexMap(HexMapSize);

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
                    int cost = 1;

                    if (noiseSample >= 0.7f)
                    {
                        //mountains
                        hexTerr = HexTerrain.MOUNTAIN;
                        cost = 5;
                    }
                    else if (noiseSample >= 0.5f)
                    {
                        //hills
                        hexTerr = HexTerrain.HILLS;
                        cost = 3;
                    }
                    else if (noiseSample >= 0.3f)
                    {
                        //grassland
                        hexTerr = HexTerrain.GRASSLAND;
                        cost = 2;
                    }
                    else
                    {
                        //lake
                        hexTerr = HexTerrain.LAKE;
                        cost = 3;
                    }

                    newCol.Add(new Hex(HexMap, i, j, cost, hexTerr, HexFeatures.NONE));
                }

                HexMap.AddColumn(newCol);
            }

            HexMap.Lock();

            GenerateMapSprites();

            ChangeLakeNames();

            ChangeMountainNames();

            ChangeRiverNamesAndDisplayRiverSprites();
        }

        private void ChangeLakeNames()
        {
            for (int i = 0; i < HexMap.Lakes.Count; i++)
            {
                foreach (var hex in HexMap.Lakes[i].Hexes)
                {
                    var hexComponent = GetHexComponent(hex);
                    hexComponent.SetNameOfFeature($"Lake {i}");
                }
            }
        }

        private void ChangeMountainNames()
        {
            for (int i = 0; i < HexMap.MountainRanges.Count; i++)
            {
                foreach (var hex in HexMap.MountainRanges[i].Hexes)
                {
                    var hexComponent = GetHexComponent(hex);
                    hexComponent.SetNameOfFeature($"Mt. {i}");
                }
            }
        }

        private void ChangeRiverNamesAndDisplayRiverSprites()
        {
            for (int i = 0; i < HexMap.Rivers.Count; i++)
            {
                for(int j = 0; j < HexMap.Rivers[i].Hexes.Count; j++)
                {
                    var hexComponent = GetHexComponent(HexMap.Rivers[i].Hexes[j]);
                    //hexComponent.SetNameOfFeature($"River {i}, {j}");
                    hexComponent.SetRiverSprites();
                }
            }
        }

        private void GenerateMapSprites()
        {
            for (int i = 0; i < HexMap.Hexes.Count; i++)
            {
                for (int j = 0; j < HexMap.Hexes[i].Count; j++)
                {
                    var newHex = HexComponent.Instance() as HexComponent;
                    newHex.Hex = HexMap.Hexes[i][j];

                    var sprite = newHex.GetNode<Sprite>(newHex.SpritePath);
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

                    HexComponentDict.Add(newHex.Hex, newHex);
                    MapNode.AddChild(newHex, true);
                }
            }
        }

        private HexComponent GetHexComponent(Hex hex)
        {
            return HexComponentDict[hex];
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

                case HexTerrain.GRASSLAND:
                    return Colors.LawnGreen;

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
