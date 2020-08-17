using Godot;
using System;
using Core;

namespace Components
{
    public class HexComponent : Node2D
    {
        public Hex Hex { get; set; }

        //called immediately after instancing
        /*public override void _EnterTree()
        {
            GD.Print($"hex component {Hex.Col}, {Hex.Row} enter tree");
        }*/

        // Called when the node enters the scene tree for the first time.
        //called after map generation has completed
        public override void _Ready()
        {
            //GD.Print($"hex component {Hex.Col}, {Hex.Row} ready");
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
