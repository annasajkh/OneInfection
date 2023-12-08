using Godot;
using OneInfection.Src.Utils;
using OneInfection.Src.ViewWindowScene;

namespace OneInfection.Src.ProjectileScene;

public partial class VirusProjectile : Area2D
{
    [Export] private ViewWindow window;
    [Export] private AnimationPlayer animationPlayer;

    public Window Window
    {
        get
        {
            return window;
        }
    }

    public float Speed { get; set; } = 200;
    public Vector2 Direction { get; set; }

    private bool shaking;

    public void Init(Vector2 position, Vector2 direction)
    {
        Position = position;
        Direction = direction;

        window.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (shaking)
        {
            Position += new Vector2I(GD.RandRange(-10, 10), GD.RandRange(-10, 10));
        }

        if (Util.IsWindowOutsideOfTheScreen(window))
        {
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
    }

    private void OnViewWindowCloseRequested()
    {
        shaking = true;
        animationPlayer.Play("window_destroyed");
    }
}