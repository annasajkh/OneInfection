using Godot;
using OneInfection.Src.DialogBoxScenes.DialogBoxScene;
using OneInfection.Src.NikoScene;
using OneInfection.Src.Utils;
using System.Collections.Generic;


namespace OneInfection.Src.MainScene;

public partial class Main : Node2D
{
    [Export] private Niko niko;
    [Export] private DialogBox dialogBox;
    [Export] private AnimationPlayer animationPlayer;
    [Export] private Node2D world;
    [Export] private Node2D virusProjectileParent;
    [Export] private Node2D virusCannonParent;
    [Export] private Node2D transparentArenaParent;
    [Export] private Timer damageOverTime;

    public bool IsMainWindowShaking { get; set; }

    private PackedScene virusCannonScene;
    private PackedScene transparentArenaScene;
    private PackedScene virusWarningScene;

    private Window mainWindow;
    private bool forceCenter = true;

    private Stack<VirusCannon> virusCannons = new Stack<VirusCannon>();

    public override void _Ready()
    {
        virusCannonScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusCannonScene/VirusCannon.tscn");
        transparentArenaScene = GD.Load<PackedScene>("res://Src/BattleScenes/TransparentArena/TransparentArena.tscn");
        virusWarningScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusWarning/VirusWarning.tscn");

        mainWindow = GetWindow();
        mainWindow.Title = "Oneshot";

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
        dialogBox.Play("something_is_wrong_with_twm");

        dialogBox.ConversationFinished += VirusInfectingTWM;
    }

    public void VirusInfectingTWM()
    {
        dialogBox.ConversationFinished -= VirusInfectingTWM;

        IsMainWindowShaking = true;

        dialogBox.Play("virus_infecting_twm");


        dialogBox.ConversationFinished += VirusTakingOverTWM;
    }

    public void VirusTakingOverTWM()
    {
        dialogBox.ConversationFinished -= VirusTakingOverTWM;

        IsMainWindowShaking = false;

        world.Modulate = new Color(1, 0, 0);
        mainWindow.Title = "OneInfection";

        DisplayServer.SetIcon(GD.Load<Texture2D>("res://icon_infected.png").GetImage());


        mainWindow.MoveToCenter();

        dialogBox.Play("virus_taking_over_twm");

        dialogBox.ConversationFinished += BattleStart;
    }

    private void BattleStart()
    {
        dialogBox.ConversationFinished -= BattleStart;

        dialogBox.Play("battle_start");
        niko.IsControlled = true;
        niko.Speed = 130;

        dialogBox.ConversationFinished += Phase1Warmup;
    }

    private void Phase1Warmup()
    {
        dialogBox.ConversationFinished -= Phase1Warmup;

        dialogBox.Play("phase_1_warmup", isAutoPlay: true);

        var virusCannon = virusCannonScene.Instantiate<VirusCannon>();

        virusCannon.Init(Util.ToWorldPosition(virusCannon.Window, new Vector2I(DisplayServer.ScreenGetSize().X / 2 - virusCannon.Window.Size.X / 2, 32)), niko, virusProjectileParent, true);
        virusCannon.Window.WindowDestroyed += Phase1End;

        virusCannonParent.AddChild(virusCannon);

        dialogBox.ConversationFinished += Phase1;
    }

    private void Phase1()
    {
        dialogBox.ConversationFinished -= Phase1;

        foreach (var child in virusCannonParent.GetChildren())
        {
            if (child is VirusCannon virusCannon)
            {
                virusCannon.VirusProjectileSpeed = 1000;
                dialogBox.Play("phase_1", isAutoPlay: true);
            }
        }
    }

    private void Phase1End()
    {
        dialogBox.ConversationFinished += Phase2;

        dialogBox.Play("phase_1_end");

        niko.Window.Visible = false;

        TransparentArena transparentArena = transparentArenaScene.Instantiate<TransparentArena>();

        transparentArena.Init(Global.WorldOutsideOffset + DisplayServer.ScreenGetSize() / 2);
        transparentArenaParent.AddChild(transparentArena);
    }

    private void Phase2()
    {
        dialogBox.ConversationFinished -= Phase2;
        dialogBox.Play("phase_2", true);

        damageOverTime.Start();


        SpawnVirusCannon(position: new Vector2I(GD.RandRange(0, DisplayServer.ScreenGetSize().X - 100), GD.RandRange(0, DisplayServer.ScreenGetSize().Y - 100)),
                         isUsingSubWindow: false);
    }

    #endregion

    private void SpawnVirusCannon(Vector2I position, bool isUsingSubWindow)
    {
        var virusCannon = virusCannonScene.Instantiate<VirusCannon>();

        Vector2I spawnPosition = Util.ToWorldPosition(virusCannon.Window, position);

        virusCannon.Init(spawnPosition, niko, virusProjectileParent, isUsingSubWindow);

        var virusWarning = virusWarningScene.Instantiate<VirusWarning>();
        virusWarning.Position = spawnPosition;
        virusCannonParent.AddChild(virusWarning);

        virusWarning.SpawnVirus += SpawnVirusCannon;

        virusCannons.Push(virusCannon);

    }

    private void SpawnVirusCannon()
    {
        virusCannonParent.AddChild(virusCannons.Pop());
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

        niko.Position = Util.ToWorldPosition(niko.Window, nikoWindowOffset);
    }

    public override void _Process(double delta)
    {
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

    private void OnDamageOverTimeTimeout()
    {
        foreach (var child in virusCannonParent.GetChildren())
        {
            if (child is VirusCannon virusCannon)
            {
                virusCannon.HealthComponent.Damage(5);
            }
        }
    }
}
