using Godot;
using System;

namespace Managers
{
    public class GameManager : Node2D
    {
        [Export]
        public NodePath MapManagerNodePath { get; set; } = new NodePath();

        public MapManager MapManager { get; private set; }

        public override void _EnterTree()
        {
            MapManager = GetNode<MapManager>(MapManagerNodePath);
            MapManager.InitialiseManager(this);

        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
