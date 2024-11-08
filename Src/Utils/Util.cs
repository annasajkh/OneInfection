﻿// Ignore Spelling: Util Utils
// Ignore Spelling: Util Utils
using Godot;

namespace OneInfection.Src.Utils;

public enum ScreenRegion
{
    Left,
    Right,
    Top,
    Bottom,
    None
}

public static class Util
{
    public static bool IsWindowOutsideOfTheScreen(Window window)
    {
        Vector2I screenSize = DisplayServer.ScreenGetSize();

        return window.Position.X - window.Size.X > screenSize.X ||
               window.Position.X + window.Size.X < 0 ||
               window.Position.Y - window.Size.Y > screenSize.Y ||
               window.Position.Y + window.Size.Y < 0;
    }

    public static Vector2I ToWorldPosition(Vector2I sizeOffset, Vector2I screenPosition)
    {
        return screenPosition + Global.WorldOutsideOffset + sizeOffset / 2;
    }

    public static Vector2I ToScreenPosition(Vector2I sizeOffset, Vector2I worldPosition)
    {

        return worldPosition - Global.WorldOutsideOffset - sizeOffset / 2;
    }
}
