using Godot;
using OneInfection.Src.Utils;
using OneInfection.Src.ViewWindowScene;

namespace OneInfection.Src.ProjectileScene;

public partial class VirusProjectile : Area2D
{
    [Export] private ViewWindow window;

    public Window Window
    {
        get
        {
            return window;
        }
    }

    public float Speed { get; set; }
    public Vector2 Direction { get; set; }


    public void Init(Vector2 position, Vector2 direction, float speed)
    {
        Position = position;
        Direction = direction;
        Speed = speed;

        window.Visible = true;
    }

    public override void _Process(double delta)
    {
        if (Util.IsWindowOutsideOfTheScreen(window))
        {
            QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Direction * Speed * (float)delta;
    }
}