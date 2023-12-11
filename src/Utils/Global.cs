// Ignore Spelling: Utils

using Godot;

namespace OneInfection.Src.Utils;


public static class Global
{
    public static Vector2I WorldOutsideOffset { get; } = new Vector2I(1000, 1000);
    public static string playerName = OS.GetEnvironment("USERNAME");
}
