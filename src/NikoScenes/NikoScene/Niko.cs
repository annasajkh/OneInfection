using Godot;

namespace OneInfection.Src.NikoScene
{
    public partial class Niko : CharacterBody2D
    {
        public Direction CurrentDirection { get; set; } = Direction.Down;
        public bool IsWalking { get; set; }
        public float Speed { get; set; } = 40;

        public bool IsOutside { get; set; }

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

        private bool isBright;

        public enum Direction
        {
            Left,
            Right,
            Up,
            Down,
        }

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
        }

        #region Signal receivers

        private void OnHitBoxAreaEntered(Area2D area)
        {
            if (area.IsInGroup("virus"))
            {
                GetTree().Quit();
            }
        }

        #endregion
    }
}
