using Godot;
using System;
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
        public Node Map { get; private set; }

        public override void InitialiseManager(GameManager gameManager)
        {
            GameManager = gameManager;
        }

        public override void _Ready()
        {
            Map = GetNode(MapNodePath);

            HexComponent = ResourceLoader.Load<PackedScene>(HexComponentPath);

            //
            for (int i = 0; i < HexMapSize; i++)
            {
                for (int j = 0; j < HexMapSize; j++)
                {
                    var newHex = HexComponent.Instance() as HexComponent;
                    newHex.Hex = new Hex(i, j);
                    var sprite = newHex.GetChild<Sprite>(0);
                    sprite.Modulate = new Color(((float)i / (float)HexMapSize), ((float)j / (float)HexMapSize), 0f);
                    var spriteSize = sprite.Texture.GetSize();
                    var pos = spriteSize * new Vector2(i, j);
                    pos.y *= 0.75f;
                    if ((j % 2) == 1)
                    {
                        pos.x -= (spriteSize.x)/2f;
                    }
                    newHex.Position = pos + (spriteSize/2f);

                    Map.AddChild(newHex, true);
                    //GD.Print($"added hex {i}, {j}");
                }
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
