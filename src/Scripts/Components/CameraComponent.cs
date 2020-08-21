using Godot;
using System;

public class CameraComponent : Camera2D
{
    private bool isBeginDrag = false;
    private Camera2D camera;
    private float zoomFactor = 0.1f;
    private readonly Vector2 ZoomInLimit = new Vector2(0.5f, 0.5f);
    private readonly Vector2 ZoomOutLimit = Vector2.One * 2f;

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
            Position += -(move.Relative * Zoom);
        }

        if (@event is InputEventMouseButton wheel)
        {
            if (wheel.ButtonIndex == (int)ButtonList.WheelUp)
            {
                Zoom += new Vector2(zoomFactor, zoomFactor);
                if (Zoom > ZoomOutLimit)
                {
                    Zoom = ZoomOutLimit;
                }
            }
            else if (wheel.ButtonIndex == (int)ButtonList.WheelDown)
            {
                Zoom -= new Vector2(zoomFactor, zoomFactor);
                if (Zoom < ZoomInLimit)
                {
                    Zoom = ZoomInLimit;
                }
            }
        }
    }
}
