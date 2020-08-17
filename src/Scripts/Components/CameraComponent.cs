using Godot;
using System;

public class CameraComponent : Node2D
{
    public override void _Process(float delta)
    {
        Position = OS.WindowSize/2f;
    }
}
