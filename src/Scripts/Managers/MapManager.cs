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
        }

        private void GenerateMap()
        {
            var noise = new OpenSimplexNoise();
            noise.Lacunarity = 350f;
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
                    else if (noiseSample >= 0.32f)
                    {
                        //grassland
                        hexTerr = HexTerrain.GRASSLAND;
                    }
                    else
                    {
                        //lake
                        hexTerr = HexTerrain.LAKE;
                    }

                    newCol.Add(new Hex(i, j, hexTerr, HexFeatures.NONE));
                }

                Map.Add(newCol);
            }
        }

        private void GenerateMapSprites()
        {
            for (int i = 0; i < Map.Count; i++)
            {
                for (int j = 0; j < Map[i].Count; j++)
                {
                    var newHex = HexComponent.Instance() as HexComponent;
                    newHex.Hex = Map[i][j];

                    var sprite = newHex.GetChild<Sprite>(0);
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

                    MapNode.AddChild(newHex, true);
                }
            }
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
