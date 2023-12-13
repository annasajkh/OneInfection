using Godot;
using OneInfection.Src.Utils;
using System;

namespace OneInfection.Src.ViewWindowScene;

public partial class ViewWindow : Window
{
    [Signal] public delegate void WindowDestroyedEventHandler();


    [Export] private Node2D positionToView;
    [Export] private Camera2D camera;
    [Export] private AnimationPlayer animationPlayer;

    [Export] private bool canBeClosed;



    private bool shaking;

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
        if (shaking)
        {
            positionToView.Position += new Vector2I(GD.RandRange(-20, 20), GD.RandRange(-20, 20));
        }

        camera.Position = positionToView.Position;
        Position = Util.ToScreenPosition(this, (Vector2I)positionToView.Position);
    }

    private void OnViewWindowCloseRequested()
    {
        shaking = true;

        if (canBeClosed)
        {
            EmitSignal(SignalName.WindowDestroyed);
            animationPlayer.Play("window_closing");
        }
    }

    private void WindowClosed()
    {
        GetParent().QueueFree();
    }

}