using Godot;
using System;
using Core;

namespace Components
{
    public class HexComponent : Node2D
    {
        public Hex Hex { get; set; }

        [Export]
        private NodePath LabelPath = new NodePath();
        public const int MAX_LABEL_LENGTH = 20;

        [Export]
        private NodePath LinesPath = new NodePath();
        private Node Lines;

        [Export]
        public NodePath SpritePath { get; set; } = new NodePath();

        public void SetNameOfFeature(string name)
        {
            if (name.Length > MAX_LABEL_LENGTH)
            {
                throw new ArgumentException($"Name of feature must be <= {MAX_LABEL_LENGTH} chars long", nameof(name));
            }

            var label = GetNode<Label>(LabelPath);
            label.Visible = true;
            label.Text = name;
        }

        public void EnableNeighbourIndicators()
        {
            Lines = GetNode(LinesPath);

            if (Hex.Neighbours.HasFlag(HexNeighbours.NorthWest))
            {
                Lines.GetNode<Line2D>(new NodePath("NW")).Visible = true;
            }

            if (Hex.Neighbours.HasFlag(HexNeighbours.NorthEast))
            {
                Lines.GetNode<Line2D>(new NodePath("NE")).Visible = true;
            }

            if (Hex.Neighbours.HasFlag(HexNeighbours.East))
            {
                Lines.GetNode<Line2D>(new NodePath("E")).Visible = true;
            }

            if (Hex.Neighbours.HasFlag(HexNeighbours.SouthEast))
            {
                Lines.GetNode<Line2D>(new NodePath("SE")).Visible = true;
            }

            if (Hex.Neighbours.HasFlag(HexNeighbours.SouthWest))
            {
                Lines.GetNode<Line2D>(new NodePath("SW")).Visible = true;
            }

            if (Hex.Neighbours.HasFlag(HexNeighbours.West))
            {
                Lines.GetNode<Line2D>(new NodePath("W")).Visible = true;
            }
        }

        //  // Called every frame. 'delta' is the elapsed time since the previous frame.
        //  public override void _Process(float delta)
        //  {
        //      
        //  }
    }
}
