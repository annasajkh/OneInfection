using Godot;
using OneInfection.Src.ViewWindowScene;

public partial class TransparentArena : Node2D
{
    [Export] private ViewWindow window;

    public void Init(Vector2 position)
    {
        window.Visible = true;
        Position = position;
    }

    public override void _Ready()
    {
        window.Size = DisplayServer.ScreenGetSize();
        window.MoveToCenter();
    }

    public override void _Process(double delta)
    {

    }
}
