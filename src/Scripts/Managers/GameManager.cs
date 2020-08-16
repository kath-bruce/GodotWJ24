using Godot;
using System;

namespace Managers
{
    public class GameManager : Node2D
    {
        // Declare member variables here. Examples:
        // private int a = 2;
        // private string b = "text";

        private bool hasStarted = false;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            hasStarted = true;
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
