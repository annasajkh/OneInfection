// Ignore Spelling: Utils

using Godot;

namespace OneInfection.src.Utils
{
    public static class Utils
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
