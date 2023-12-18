using Godot;
using OneInfection.Src.DialogBoxScenes.OutsideDialogBoxScene;
using OneInfection.Src.ViewWindowScene;

namespace OneInfection.Src.NikoScene;

public enum Direction
{
    Left,
    Right,
    Up,
    Down,
}

public partial class Niko : CharacterBody2D
{
    [Export] private OutsideDialogBox outsideDialogBox;

    public Direction CurrentDirection { get; set; } = Direction.Down;
    public bool IsWalking { get; set; }
    public float Speed { get; set; } = 75;

    public bool IsOutside { get; set; }

    public ViewWindow Window
    {
        get
        {
            return window;
        }
    }

    [Export] public bool IsControlled { get; set; }

    [Export]
    public bool IsBright
    {
        get
        {
            return isBright;
        }
        set
        {
            isBright = value;

            if (isBright)
            {
                spriteSheet.Texture = GD.Load<Texture2D>("res://assets/textures/niko/niko_bright_sheet.png");
            }
            else
            {
                spriteSheet.Texture = GD.Load<Texture2D>("res://assets/textures/niko/niko_dim_sheet.png");
            }
        }
    }

    [Export] private AnimationPlayer animationPlayer;
    [Export] private Sprite2D spriteSheet;
    [Export] private ViewWindow window;

    private bool isBright;

    public override void _Ready()
    {
        IsControlled = false;
        IsBright = true;
    }

    public void Face(Direction direction)
    {
        CurrentDirection = direction;
    }

    public void AutoWalk(Direction direction, float time)
    {
        IsWalking = true;

        CurrentDirection = direction;

        GetTree().CreateTimer(time).Timeout += () =>
        {
            IsWalking = false;
        };
    }

    private void KeyboardInput()
    {
        if (Input.IsActionPressed("walk_left"))
        {
            IsWalking = true;
            CurrentDirection = Direction.Left;
        }
        else if (Input.IsActionPressed("walk_right"))
        {
            IsWalking = true;
            CurrentDirection = Direction.Right;
        }
        else if (Input.IsActionPressed("walk_up"))
        {
            IsWalking = true;
            CurrentDirection = Direction.Up;
        }
        else if (Input.IsActionPressed("walk_down"))
        {
            IsWalking = true;
            CurrentDirection = Direction.Down;
        }
        else
        {
            IsWalking = false;
        }
    }

    private void Act()
    {
        Vector2 moveDirection = Vector2.Zero;

        switch (CurrentDirection)
        {
            case Direction.Left:
                if (IsWalking)
                {
                    animationPlayer.Play("walk_left");
                    moveDirection = Vector2.Left;
                }
                else
                {
                    animationPlayer.Play("idle_left");
                    moveDirection = Vector2.Zero;
                }
                break;

            case Direction.Right:
                if (IsWalking)
                {
                    animationPlayer.Play("walk_right");
                    moveDirection = Vector2.Right;
                }
                else
                {
                    animationPlayer.Play("idle_right");
                    moveDirection = Vector2.Zero;
                }
                break;

            case Direction.Up:
                if (IsWalking)
                {
                    animationPlayer.Play("walk_up");
                    moveDirection = Vector2.Up;
                }
                else
                {
                    animationPlayer.Play("idle_up");
                    moveDirection = Vector2.Zero;
                }
                break;

            case Direction.Down:
                if (IsWalking)
                {
                    animationPlayer.Play("walk_down");
                    moveDirection = Vector2.Down;
                }
                else
                {
                    animationPlayer.Play("idle_down");
                    moveDirection = Vector2.Zero;
                }
                break;
        }

        Velocity = moveDirection * Speed;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (IsControlled)
        {
            KeyboardInput();
        }

        Act();

        MoveAndSlide();


        // Prevent niko from going outside screen boundary
        if (IsOutside)
        {
            Vector2I screenSize = DisplayServer.ScreenGetSize();

            if (window.Position.X + window.Size.X / 2 + 24 > screenSize.X)
            {
                float collisionDepth = Mathf.Abs(screenSize.X - (window.Position.X + window.Size.X / 2 + 24));

                Position += Vector2.Left * collisionDepth;
            }
            else if (window.Position.X + window.Size.X / 2 - 24 < 0)
            {
                float collisionDepth = Mathf.Abs(window.Position.X + window.Size.X / 2 - 24);

                Position += Vector2.Right * collisionDepth;
            }
            else if (window.Position.Y + window.Size.Y / 2 + 32 > screenSize.Y)
            {
                float collisionDepth = Mathf.Abs(screenSize.Y - (window.Position.Y + window.Size.Y / 2 + 32));

                Position += Vector2.Up * collisionDepth;
            }
            else if (window.Position.Y + window.Size.Y / 2 - 32 < 0)
            {
                float collisionDepth = Mathf.Abs(window.Position.Y + window.Size.Y / 2 - 32);

                Position += Vector2.Down * collisionDepth;
            }
        }

    }

    private void OnHitBoxAreaEntered(Area2D area)
    {
        if (area.IsInGroup("virus"))
        {
            GetTree().Quit();
        }
    }
}
