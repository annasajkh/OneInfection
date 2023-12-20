using Godot;
using OneInfection.Src.BattleScenes.VirusProjectileScene;
using OneInfection.Src.Components.HealthComponent;
using OneInfection.Src.ViewWindowScene;

public partial class VirusCannon : Node2D
{
    [Export] private Node2D cannonPivot;
    [Export] private Timer virusProjectileTimer;
    [Export] private AnimationPlayer animationPlayer;
    [Export] private VirusHealthBarComponent virusHealthBarComponent;
    [Export] private GpuParticles2D deathParticle;

    public VirusHealthBarComponent VirusHealthBarComponent
    {
        get
        {
            return virusHealthBarComponent;
        }
    }

    public Timer VirusProjectileTimer
    {
        get
        {
            return virusProjectileTimer;
        }
    }

    [Export] private HealthComponent healthComponent;
    public HealthComponent HealthComponent
    {
        get
        {
            return healthComponent;
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

    [Export] private Sprite2D virusCannonCarrier;
    public Sprite2D VirusCannonCarrier
    {
        get
        {
            return virusCannonCarrier;
        }
    }

    public float VirusProjectileSpeed { get; set; } = 600;

    public Node2D Target { get; private set; }
    public Node2D VirusProjectileParent { get; private set; }

    private PackedScene virusProjectileScene;

    private bool isUsingSubWindow;
    private float moveSpeed = 30;

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
        if (isUsingSubWindow)
        {
            virusHealthBarComponent.Visible = false;
        }

        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusProjectileScene/VirusProjectile.tscn");

        animationPlayer.Play("fire");

        if (!isUsingSubWindow)
        {
            deathParticle.Finished += QueueFree;
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

    public override void _Process(double delta)
    {
        if (Target != null)
        {
            Vector2 targetDirection = (Target.Position - cannonPivot.GlobalPosition).Normalized();

            cannonPivot.Rotation = (float)(targetDirection.Angle() - Mathf.DegToRad(90));

            if (!isUsingSubWindow)
            {
                Position += targetDirection * moveSpeed * (float)delta;
            }
        }
    }

    #region Signal receivers

    private void OnVirusProjectileTimerTimeout()
    {
        animationPlayer.Play("fire");
    }

    private void OnHealthComponentDied()
    {
        animationPlayer.Play("died");
    }

    #endregion
}
