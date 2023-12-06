using Godot;

namespace OneInfection.Src.BattleScene;


public class Projectile
{
    public float Radius { get; private set; }
    public Vector2 Speed { get; private set; }
    public Vector2I Coord { get; private set; }

    private bool isSpeedDecreasing = false;

    public Projectile(Vector2I start, float radius, bool isSpeedDecreasing)
    {
        Radius = radius;
        Coord = start;
        this.isSpeedDecreasing = isSpeedDecreasing;
    }

    public bool IsDestroyable()
    {
        var res = DisplayServer.WindowGetSize();
        return
            (Radius < 0.1f) ||
            ((Coord.X - Radius) > res.X) ||
            ((Coord.Y - Radius) > res.Y) ||
            (Coord.X + Radius) < 0 ||
            (Coord.Y + Radius) < 0;
    }
}
