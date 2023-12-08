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

    public bool IsMainWindowShaking { get; set; }

    private Window mainWindow;
    private PackedScene virusProjectileScene;
    private bool forceCenter = true;

    public override void _Ready()
    {
        virusProjectileScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusProjectileScene/VirusProjectile.tscn");

        mainWindow = GetWindow();

        PlayMainDialogChain();
    }

    #region Main dialog chain
    public void PlayMainDialogChain()
    {
        dialogBox.Play("goodbye");
        dialogBox.ConversationFinished += GoodbyeConversation;
    }

    private void GoodbyeConversation()
    {
        dialogBox.ConversationFinished -= GoodbyeConversation;

        animationPlayer.Play("goodbye_niko");
    }

    public void SomethingIsWrongWithTWM()
    {
        dialogBox.Play("something_is_wrong_with_TWM");

        dialogBox.ConversationFinished += VirusInfectingTWM;
    }

    public void VirusInfectingTWM()
    {
        dialogBox.ConversationFinished -= VirusInfectingTWM;

        IsMainWindowShaking = true;

        dialogBox.Play("virus_infecting_TWM");


        dialogBox.ConversationFinished += VirusTakingOverTWM;
    }

    public void VirusTakingOverTWM()
    {
        dialogBox.ConversationFinished -= VirusTakingOverTWM;

        IsMainWindowShaking = false;

        world.Modulate = new Color(1, 0, 0);
        mainWindow.Title = "OneInfection";

        DisplayServer.SetIcon(Image.LoadFromFile("res://icon_infected.png"));


        mainWindow.MoveToCenter();

        dialogBox.Play("virus_taking_over_TWM");

        dialogBox.ConversationFinished += BattleStart;
    }

    private void BattleStart()
    {
        dialogBox.ConversationFinished -= BattleStart;

        dialogBox.Play("battle_start");

        niko.IsControlled = true;

        dialogBox.ConversationFinished += VirusProjectileAttack;
    }

    private void VirusProjectileAttack()
    {
        dialogBox.ConversationFinished -= VirusProjectileAttack;

        SpawnVirusProjectile();
        animationPlayer.Play("virus_projectile_attack");
    }

    #endregion

    public void SpawnVirusProjectile()
    {
        VirusProjectile virusProjectile = virusProjectileScene.Instantiate<VirusProjectile>();

        Vector2 projectilePosition = Util.ToWorldPositionFromScreenWindowCenteredPosition(virusProjectile.Window, DisplayServer.ScreenGetSize() / 2);


        virusProjectile.Init(projectilePosition, (niko.Position - projectilePosition).Normalized());

        world.AddChild(virusProjectile);
    }

    private void OnFirstHouseGoOutside()
    {
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
