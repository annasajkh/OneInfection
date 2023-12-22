using Godot;

public partial class VirusWarning : Sprite2D
{

    [Signal] public delegate void SpawnVirusEventHandler();

    [Export] private Timer timer;

    [Export] private MeshInstance2D meshInstance2D;

    public void Init(float waitTime, Vector2 scale)
    {
        timer.WaitTime = waitTime;
        meshInstance2D.Scale = scale;
    }

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
