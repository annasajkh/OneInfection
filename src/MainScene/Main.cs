using Godot;
using OneInfection.Src.DialogBoxScenes.DialogBoxScene;
using OneInfection.Src.NikoScenes.NikoScene;
using OneInfection.Src.ProjectileScene;
using OneInfection.Src.Utils;

namespace OneInfection.Src.MainScene;


public partial class Main : Node2D
{
    [Export] private Niko niko;
    [Export] private DialogBox dialogBox;
    [Export] private AnimationPlayer animationPlayer;
    [Export] private Node2D world;
    [Export] private Timer virusProjectileTimer;

    public bool IsMainWindowShaking { get; set; }

    private Window mainWindow;
    private PackedScene virusProjectileScene;
    private bool forceCenter = true;

    public override void _Ready()
    {
        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScene/ProjectileScene/Projectile.tscn");

        mainWindow = GetWindow();

        PlayMainDialogChain();
    }

    #region Main dialog chain
    public void PlayMainDialogChain()
    {
        dialogBox.Play(DialogParser.Parse("assets/dialogs/Goodbye.json"));
        dialogBox.ConversationFinished += GoodbyeConversation;
    }

    private void GoodbyeConversation()
    {
        dialogBox.ConversationFinished -= GoodbyeConversation;

        animationPlayer.Play("goodbye_niko");
    }

    public void SomethingIsWrongWithTWM()
    {
        dialogBox.Play(DialogParser.Parse("assets/dialogs/SomethingIsWrongWithTWM.json"));

        dialogBox.ConversationFinished += VirusInfectingTWM;
    }

    public void VirusInfectingTWM()
    {
        dialogBox.ConversationFinished -= VirusInfectingTWM;

        IsMainWindowShaking = true;

        dialogBox.Play(DialogParser.Parse("assets/dialogs/VirusInfectingTWM.json"));


        dialogBox.ConversationFinished += VirusTakingOverTWM;
    }

    public void VirusTakingOverTWM()
    {
        dialogBox.ConversationFinished -= VirusTakingOverTWM;

        IsMainWindowShaking = false;

        mainWindow.Title = "OneInfection";
        DisplayServer.SetIcon(Image.LoadFromFile("res://icon_infected.png"));


        mainWindow.MoveToCenter();

        dialogBox.Play(DialogParser.Parse("assets/dialogs/VirusTakingOverTWM.json"));
        dialogBox.ConversationFinished += BattleStart;
    }

    private void BattleStart()
    {
        SpawnVirusProjectile();
        //virusProjectileTimer.Start();

        dialogBox.ConversationFinished -= BattleStart;
        dialogBox.Play(DialogParser.Parse("assets/dialogs/VirusRamblingAtBattle.json"));

        niko.IsControlled = true;
    }
    #endregion

    public void SpawnVirusProjectile()
    {
        Projectile projectile = virusProjectileScene.Instantiate<Projectile>();

        projectile.Init(position: Util.ToWorldPositionFromScreenWindowCenteredPosition(projectile.Window, DisplayServer.ScreenGetSize() / 2),
                        direction: Vector2.Right.Rotated((float)GD.RandRange(0.0, Mathf.Tau)),
                        speed: 10);

        world.AddChild(projectile);
    }

    private void OnFirstHouseGoOutside()
    {
        niko.Speed *= 2;

        Vector2I initialWindowPosition = mainWindow.Position;
        Vector2I initialWindowSize = mainWindow.Size;

        Vector2I nikoWindowOffset = initialWindowPosition;

        nikoWindowOffset.X += initialWindowSize.X / 2 - niko.Window.Size.X / 2 + 10;
        nikoWindowOffset.Y += initialWindowSize.Y - niko.Window.Size.Y / 2 - 32;

        niko.IsBright = false;
        niko.Window.Visible = true;
        niko.IsOutside = true;

        niko.Position = Util.ToWorldPositionFromScreenWindowCenteredPosition(niko.Window, nikoWindowOffset);
    }

    private void OnVirusProjectileTimerTimeout()
    {
        SpawnVirusProjectile();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("skip"))
        {
            dialogBox.Skip();
        }

        if (IsMainWindowShaking)
        {
            mainWindow.Position += new Vector2I(GD.RandRange(-20, 20), GD.RandRange(-20, 20));

            if (GD.RandRange(0, 3) == 0)
            {
                mainWindow.MoveToCenter();
            }
        }
        else if (forceCenter)
        {
            mainWindow.MoveToCenter();
        }
    }
}
