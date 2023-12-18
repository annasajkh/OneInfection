using Godot;
using OneInfection.Src.Utils;
using OneInfection.Src.ViewWindowScene;

namespace OneInfection.Src.BattleScenes.VirusProjectileScene;

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

    public void Init(Vector2 position, Vector2 direction, float speed, bool isUsingSubWindow)
    {
        Position = position;
        Direction = direction;
        Speed = speed;

        if (!isUsingSubWindow)
        {
            Window.Visible = false;
        }
        else
        {
            Window.Visible = true;
        }
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
