using Godot;

namespace OneInfection.Src.Components.HealthComponent;

public partial class HealthComponent : Node
{
    [Export] public float MaxHealth { get; private set; }

    public float Health { get; private set; }

    [Signal] public delegate void DiedEventHandler();
    [Signal] public delegate void DamagedEventHandler();

    public override void _Ready()
    {
        Health = MaxHealth;
    }

    public void Damage(float damage)
    {
        EmitSignal(SignalName.Damaged);

        Health -= damage;

        if (Health <= 0)
        {
            EmitSignal(SignalName.Died);
        }
    }
}
