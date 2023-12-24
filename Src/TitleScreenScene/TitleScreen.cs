using Godot;

public partial class TitleScreen : Control
{
    private void OnStartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Src/InstructionScene/Instruction.tscn");
    }

    private void OnAboutButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Src/AboutScene/About.tscn");
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
