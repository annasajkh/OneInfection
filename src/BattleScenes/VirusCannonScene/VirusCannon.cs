using Godot;
using OneInfection.Src.ProjectileScene;
using OneInfection.Src.ViewWindowScene;

public partial class VirusCannon : Node2D
{
    [Export] private Node2D cannonPivot;
    [Export] private Timer virusProjectileTimer;
    [Export] private AnimationPlayer animationPlayer;

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
    public float VirusProjectileSpeed { get; set; } = 500;

    public Node2D Target { get; private set; }
    public Node2D VirusProjectileParent { get; private set; }

    private PackedScene virusProjectileScene;

    public void Init(Vector2I position, Node2D target, Node2D virusProjectileParent)
    {
        Position = position;
        Target = target;
        VirusProjectileParent = virusProjectileParent;
    }

    public override void _Ready()
    {
        Window.Visible = true;

        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusProjectileScene/VirusProjectile.tscn");

        animationPlayer.Play("fire");
    }

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            cannonPivot.Rotation = (float)(Target.Position - cannonPivot.GlobalPosition).Normalized().Angle() - Mathf.DegToRad(90);
        }
    }

    private void Fire()
    {
        VirusProjectile virusProjectile = virusProjectileScene.Instantiate<VirusProjectile>();

        virusProjectile.Init(cannonPivot.GlobalPosition, (Target.Position - cannonPivot.GlobalPosition).Normalized(), VirusProjectileSpeed);

        VirusProjectileParent.AddChild(virusProjectile);
    }

    private void OnVirusProjectileTimerTimeout()
    {
        animationPlayer.Play("fire");
    }
}
