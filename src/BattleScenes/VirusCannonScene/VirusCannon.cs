using Godot;
using OneInfection.Src.ProjectileScene;
using OneInfection.Src.ViewWindowScene;

public partial class VirusCannon : Node2D
{
    [Signal] public delegate void DestroyedEventHandler();

    [Export] private Timer virusProjectileTimer;

    public Timer VirusProjectileTimer
    {
        get
        {
            return virusProjectileTimer;
        }
    }

    [Export] private ViewWindow window;

    public ViewWindow Window
    {
        get
        {
            return window;
        }
    }

    public Node2D Target { get; private set; }
    public Node2D VirusProjectileParent { get; private set; }

    private float virusProjectileSpeed = 500;

    private PackedScene virusProjectileScene;

    public void Init(Vector2I position, Node2D target, Node2D virusProjectileParent)
    {
        Position = position;
        Target = target;
        VirusProjectileParent = virusProjectileParent;

        Window.WindowDestroyed += () =>
        {
            EmitSignal(SignalName.Destroyed);
        };
    }

    public override void _Ready()
    {
        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusProjectileScene/VirusProjectile.tscn");
        Fire();
    }

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            Rotation = (float)(Target.Position - Position).Normalized().Angle() - Mathf.DegToRad(90);
        }
    }

    private void Fire()
    {
        VirusProjectile virusProjectile = virusProjectileScene.Instantiate<VirusProjectile>();

        virusProjectile.Init(Position, (Target.Position - Position).Normalized(), virusProjectileSpeed);

        VirusProjectileParent.AddChild(virusProjectile);
    }

    private void OnVirusProjectileTimerTimeout()
    {
        Fire();
    }
}
