// Ignore Spelling: Util Utils
// Ignore Spelling: Util Utils
using Godot;

namespace OneInfection.Src.Utils;


public static class Util
{
    public static Vector2I ScreenSize { get; } = DisplayServer.ScreenGetSize();

    public static bool IsWindowOutsideOfTheScreen(Window window)
    {
        return window.Position.X - window.Size.X > ScreenSize.X ||
               window.Position.X + window.Size.X < 0 ||
               window.Position.Y - window.Size.Y > ScreenSize.Y ||
               window.Position.Y + window.Size.Y < 0;
    }

    public static Vector2I ToWorldPosition(Vector2I screenPosition)
    {
        return screenPosition + Global.WorldOutsideOffset;
    }

    public static Vector2I ToScreenPosition(Vector2I worldPosition)
    {
        return worldPosition - Global.WorldOutsideOffset;
    }
}
