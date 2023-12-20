using Godot;

public partial class Instruction : Control
{
    [Export] AnimationPlayer animationPlayer;

    private bool isEnterAlreadyPressed;

    public override void _Ready()
    {

    }

    public void ChangeToMainScene()
    {
        GetTree().ChangeSceneToFile("res://Src/MainScene/Main.tscn");
    }

    public override void _Process(double delta)
    {
        if (Input.IsKeyPressed(Key.Enter) && !isEnterAlreadyPressed)
        {
            animationPlayer.Play("change_scene");
            isEnterAlreadyPressed = true;
        }
    }
}
