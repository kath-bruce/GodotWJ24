using Godot;
using System;
using Core;

namespace Components
{
    public class HexComponent : Node2D
    {
        public const int MAX_LABEL_LENGTH = 20;
        public Hex Hex { get; set; }

        [Export]
        private NodePath LinesPath = new NodePath();
        private Node Lines;

        //called immediately after instancing
        /*public override void _EnterTree()
        {
            GD.Print($"hex component {Hex.Col}, {Hex.Row} enter tree");
            Lines = GetNode(LinesPath);
            GD.Print($"lines node assigned?: {Lines != null}");

        }*/

        // Called when the node enters the scene tree for the first time.
        //called after map generation has completed
        /*public override void _Ready()
        {
            GD.Print($"hex component {Hex.Col}, {Hex.Row} ready");
            Lines = GetNode(LinesPath);
            GD.Print($"lines node assigned?: {Lines != null}");
        }*/

        public void SetNameOfFeature(string name)
        {
            if (name.Length > MAX_LABEL_LENGTH)
            {
                throw new ArgumentException($"Name of feature must be <= {MAX_LABEL_LENGTH} chars long", nameof(name));
            }

            var label = GetNode<Label>("HexLabel");
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
