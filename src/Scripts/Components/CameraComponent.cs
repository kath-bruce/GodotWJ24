using Godot;
using System;

public class CameraComponent : Node2D
{
    private bool isBeginDrag = false;

    public override void _Ready()
    {
        Position = OS.WindowSize / 2f;
    }

    public override void _Input(InputEvent @event)
    {
        if ((@event is InputEventMouseButton click) && click.ButtonIndex == (int)ButtonList.Left)
        {
            isBeginDrag = click.Pressed;
        }

        if (@event is InputEventMouseMotion move && isBeginDrag)
        {
            Position += -move.Relative;
        }
    }
}
