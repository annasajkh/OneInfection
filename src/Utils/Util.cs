// Ignore Spelling: Util Utils

using Godot;


namespace OneInfection.Src.Utils
{
    public static class Util
    {
        public static Vector2I ToWorldPosition(Vector2I screenPosition)
        {
            return screenPosition + Global.WorldOutsideOffset;
        }

        public static Vector2I ToScreenPosition(Vector2I worldPosition)
        {
            return worldPosition - Global.WorldOutsideOffset;
        }
    }
}
