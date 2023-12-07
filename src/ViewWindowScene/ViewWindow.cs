using Godot;
using OneInfection.Src.Utils;
using System;

namespace OneInfection.Src.ViewWindowScene;

public partial class ViewWindow : Window
{
    [Export] private Node2D positionToView;
    [Export] private Camera2D camera;

    public override void _Ready()
    {
        World2D = GetTree().Root.World2D;

        if (positionToView == null)
        {
            throw new Exception("Please set the position for this window to view in the godot editor");
        }
    }

    public override void _Process(double delta)
    {
        camera.Position = positionToView.Position;
        Position = Util.ToScreenWindowCenteredPosition(this, (Vector2I)positionToView.Position);
    }

}