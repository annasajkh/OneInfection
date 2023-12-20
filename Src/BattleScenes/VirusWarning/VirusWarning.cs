using Godot;

public partial class VirusWarning : Sprite2D
{

    [Signal] public delegate void SpawnVirusEventHandler();

    public override void _Ready()
    {
    }


    public override void _Process(double delta)
    {

    }

    private void OnTimerTimeout()
    {
        EmitSignal(SignalName.SpawnVirus);
        QueueFree();
    }
}
