using Godot;
using OneInfection.Src.Components.HealthComponent;

public partial class VirusHealthBarComponent : Node2D
{
    [Export] private HealthComponent healthComponent;
    [Export] private MeshInstance2D meshInstance2D;


    private Vector2 healthBarScale;

    public override void _Ready()
    {
        healthBarScale = meshInstance2D.Scale;
    }

    public override void _Process(double delta)
    {
        meshInstance2D.Scale = new Vector2(healthBarScale.X * (healthComponent.Health / healthComponent.MaxHealth), healthBarScale.Y);
    }
}