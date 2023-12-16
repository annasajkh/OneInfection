using Godot;
using OneInfection.Src.DialogBoxScenes.DialogBoxScene;
using OneInfection.Src.NikoScenes.NikoScene;
using OneInfection.Src.Utils;


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

    public bool IsMainWindowShaking { get; set; }

    private PackedScene virusCannonScene;
    private PackedScene transparentArenaScene;

    private Window mainWindow;
    private bool forceCenter = true;

    public override void _Ready()
    {
        virusCannonScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusCannonScene/VirusCannon.tscn");
        transparentArenaScene = GD.Load<PackedScene>("res://Src/BattleScenes/TransparentArena/TransparentArena.tscn");

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

        DisplayServer.SetIcon(GD.Load<Texture2D>("res://icon_infected.png").GetImage());


        mainWindow.MoveToCenter();

        dialogBox.Play("virus_taking_over_TWM");

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

        var virusCannon = virusCannonScene.Instantiate<VirusCannon>();

        virusCannon.VirusProjectileTimer.WaitTime = 1f;

        virusCannon.Init(Util.ToWorldPosition(virusCannon.Window, new Vector2I(DisplayServer.ScreenGetSize().X / 2 - virusCannon.Window.Size.X / 2, 32)), niko, virusProjectileParent, false);

        virusCannonParent.AddChild(virusCannon);
    }

    #endregion

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
