using Godot;

public partial class About : Control
{
    private void OnBackButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://Src/TitleScreenScene/TitleScreen.tscn");
    }
}
