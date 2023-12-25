using Godot;
using OneInfection.Src.BattleScenes.VirusProjectileScene;

public partial class VirusBomb : Area2D
{
    public Node2D VirusProjectileParent { get; private set; }

    private PackedScene virusProjectileScene;

    [Export] private AudioStreamPlayer bombExplodeSound;

    private float virusProjectileSpeed;

    public void Init(Vector2I position, Node2D virusProjectileParent, float virusProjectileSpeed)
    {
        Position = position;
        VirusProjectileParent = virusProjectileParent;
        this.virusProjectileSpeed = virusProjectileSpeed;
    }

    public override void _Ready()
    {
        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusProjectileScene/VirusProjectile.tscn");
    }

    public override void _Process(double delta)
    {

    }

    private void OnCountDownTimerTimeout()
    {
        int projectileCount = 8;

        for (int i = 0; i < 360; i += 360 / projectileCount)
        {
            var virusProjectile = virusProjectileScene.Instantiate<VirusProjectile>();

            virusProjectile.Init(Position, Vector2.One.Rotated(Mathf.DegToRad(i)), virusProjectileSpeed, false);

            VirusProjectileParent.AddChild(virusProjectile);
        }

        Visible = false;
        bombExplodeSound.Play();

        bombExplodeSound.Finished += () =>
        {
            QueueFree();
        };
    }
}
