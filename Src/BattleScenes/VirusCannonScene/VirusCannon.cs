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
    public float VirusProjectileSpeed { get; set; } = 600;

    public Node2D Target { get; private set; }
    public Node2D VirusProjectileParent { get; private set; }

    private PackedScene virusProjectileScene;

    private bool isUsingSubWindow;

    public void Init(Vector2I position, Node2D target, Node2D virusProjectileParent, bool isUsingSubWindow)
    {
        this.isUsingSubWindow = isUsingSubWindow;

        Position = position;
        Target = target;
        VirusProjectileParent = virusProjectileParent;

        if (!isUsingSubWindow)
        {
            Window.Visible = false;
        }
        else
        {
            Window.Visible = true;
        }
    }

    public override void _Ready()
    {
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

        if (!isUsingSubWindow)
        {
            virusProjectile.Window.Visible = false;
        }

        virusProjectile.Init(cannonPivot.GlobalPosition, (Target.Position - cannonPivot.GlobalPosition).Normalized(), VirusProjectileSpeed, isUsingSubWindow);

        VirusProjectileParent.AddChild(virusProjectile);
    }

    private void OnVirusProjectileTimerTimeout()
    {
        animationPlayer.Play("fire");
    }
}
