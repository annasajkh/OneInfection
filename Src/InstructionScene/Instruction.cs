using Godot;

public partial class Instruction : Control
{
    [Export] AnimationPlayer animationPlayer;

    private bool isEnterAlreadyPressed;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void ChangeToMainScene()
    {
        GetTree().ChangeSceneToFile("res://Src/MainScene/Main.tscn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Enter) && !isEnterAlreadyPressed)
        {
            animationPlayer.Play("change_scene");
            isEnterAlreadyPressed = true;
        }
    }
}
