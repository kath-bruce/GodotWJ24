using Godot;
using System;
using Managers;

public class CameraComponent : Camera2D
{
    private bool isBeginDrag = false;
    private Camera2D camera;
    private readonly float zoomFactor = 0.1f;
    private readonly Vector2 ZoomInLimit = new Vector2(0.5f, 0.5f);
    private readonly Vector2 ZoomOutLimit = Vector2.One * 2f;

    [Export]
    public NodePath GameManagerNodePath = new NodePath();

    private GameManager GameManager;

    public override void _Ready()
    {
        Position = OS.WindowSize / 2f;
        GameManager = GetNode<GameManager>(GameManagerNodePath);
    }

    public override void _Process(float delta)
    {
        MoveCameraInsideBounds();
    }

    private void MoveCameraInsideBounds()
    {
        var bottomRightCorner = GameManager.MapManager.GetBottomRightCornerOfMapInPixelCoords();

        var margin = (OS.WindowSize / 2f) * Zoom;

        if (Position.x < (0f + margin.x) && Position.x < (bottomRightCorner.x - margin.x))
        {
            var newPos = Position;
            newPos.x = (0f + margin.x);
            Position = newPos;
        }
        else if (Position.x > (bottomRightCorner.x - margin.x) && Position.x > (0f + margin.x))
        {
            var newPos = Position;
            newPos.x = (bottomRightCorner.x - margin.x);
            Position = newPos;
        }

        if (Position.y < (0f + margin.y) && Position.y < (bottomRightCorner.y - margin.y))
        {
            var newPos = Position;
            newPos.y = (0f + margin.y);
            Position = newPos;
        }
        else if (Position.y > (bottomRightCorner.y - margin.y) && Position.y > (0f + margin.y))
        {
            var newPos = Position;
            newPos.y = (bottomRightCorner.y - margin.y);
            Position = newPos;
        }
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

            MoveCameraInsideBounds();
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
