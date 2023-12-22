using Godot;
using OneInfection.Src.Utils;

namespace OneInfection.Src.BattleScenes.VirusHandScene;

public enum VirusHandMoveDir
{
    LeftRight,
    RightLeft,
    TopDown,
    DownTop
}

public partial class VirusHand : Area2D
{
    private VirusHandMoveDir moveDir;

    private float speed = 500;

    public void Init(VirusHandMoveDir moveDir)
    {
        this.moveDir = moveDir;

        var screenSize = DisplayServer.ScreenGetSize();

        switch (moveDir)
        {
            case VirusHandMoveDir.LeftRight:
                Position = new Vector2I(-128 + Global.WorldOutsideOffset.X, (int)(128 * 0.6f) + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.RightLeft:
                Position = new Vector2I(screenSize.X + 128 + Global.WorldOutsideOffset.X, screenSize.Y - 120 + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.TopDown:
                Position = new Vector2I(Global.WorldOutsideOffset.X + screenSize.X - 128, -128 + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.DownTop:
                Position = new Vector2I(Global.WorldOutsideOffset.X + 128, screenSize.Y + Global.WorldOutsideOffset.Y);
                break;
        }
    }

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero;

        switch (moveDir)
        {
            case VirusHandMoveDir.LeftRight:
                velocity = Vector2.Right;
                break;

            case VirusHandMoveDir.RightLeft:
                velocity = Vector2.Left;
                break;

            case VirusHandMoveDir.TopDown:
                velocity = Vector2.Down;
                break;

            case VirusHandMoveDir.DownTop:
                velocity = Vector2.Up;
                break;
        }

        Position += velocity * speed * (float)delta;
    }

    private void OnTimerTimeout()
    {
        QueueFree();
    }
}
