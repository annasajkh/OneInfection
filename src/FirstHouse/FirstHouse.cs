using Godot;

public partial class FirstHouse : Node2D
{
    [Signal]
    public delegate void GoOutsideEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    private void OnGoOutsideBodyEntered(Node2D body)
    {
        if (body.Name == "Niko")
        {
            EmitSignal(SignalName.GoOutside);
        }
    }
}
