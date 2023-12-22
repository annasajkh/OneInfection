using Godot;
using OneInfection.Src.BattleScenes.VirusHandScene;
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
    [Export] private Node2D virusParent;
    [Export] private Node2D transparentArenaParent;
    [Export] private Timer damageOverTime;
    [Export] private Timer virusHandSpawnTimer;
    [Export] private Timer virusCannonSpawnTimer;

    public bool IsMainWindowShaking { get; set; }

    private PackedScene virusCannonScene;
    private PackedScene virusHandScene;

    private PackedScene transparentArenaScene;
    private PackedScene virusWarningScene;

    private Window mainWindow;
    private bool forceCenter = true;

    private Stack<VirusCannon> virusCannons = new Stack<VirusCannon>();
    private Stack<VirusHand> virusHands = new Stack<VirusHand>();

    private static VirusHandMoveDir[] virusHandMoveDirs = new VirusHandMoveDir[] { VirusHandMoveDir.LeftRight, VirusHandMoveDir.RightLeft, VirusHandMoveDir.TopDown, VirusHandMoveDir.DownTop };

    public override void _Ready()
    {
        virusCannonScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusCannonScene/VirusCannon.tscn");
        virusHandScene = GD.Load<PackedScene>("res://Src/BattleScenes/VirusHandScene/VirusHand.tscn");

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

        virusParent.AddChild(virusCannon);

        dialogBox.ConversationFinished += Phase1;
    }

    private void Phase1()
    {
        dialogBox.ConversationFinished -= Phase1;

        foreach (var child in virusParent.GetChildren())
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

        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 600);


        dialogBox.ConversationFinished += Phase2End;
    }

    private void Phase2End()
    {
        dialogBox.ConversationFinished -= Phase2End;

        dialogBox.Play("phase_2_end", true);

        virusHandSpawnTimer.Start();
        virusCannonSpawnTimer.Start();

        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
    }

    #endregion


    private void SpawnVirusCannon(bool isUsingSubWindow, float virusProjectileSpeed = 600)
    {
        var virusCannon = virusCannonScene.Instantiate<VirusCannon>();
        var virusWarning = virusWarningScene.Instantiate<VirusWarning>();

        virusCannon.VirusProjectileSpeed = virusProjectileSpeed;

        virusWarning.Init(2, new Vector2(128, 128));

        Vector2I spawnPosition = new Vector2I(GD.RandRange(320 / 2, DisplayServer.ScreenGetSize().X - 320 / 2), GD.RandRange(320 / 2, DisplayServer.ScreenGetSize().Y - 320 / 2));

        while ((spawnPosition - Util.ToScreenPosition(niko.Window, (Vector2I)niko.Position)).Length() < 750)
        {
            spawnPosition = new Vector2I(GD.RandRange(320 / 2, DisplayServer.ScreenGetSize().X - 320 / 2), GD.RandRange(320 / 2, DisplayServer.ScreenGetSize().Y - 320 / 2));
        }

        spawnPosition = Util.ToWorldPosition(virusCannon.Window, spawnPosition);

        virusWarning.Position = spawnPosition;
        virusCannon.Init(spawnPosition, niko, virusProjectileParent, isUsingSubWindow);


        virusParent.AddChild(virusWarning);
        virusWarning.SpawnVirus += ActuallySpawnVirusCannon;
        virusCannons.Push(virusCannon);

    }

    private void ActuallySpawnVirusCannon()
    {
        virusParent.AddChild(virusCannons.Pop());
    }


    private void SpawnVirusHand(VirusHandMoveDir moveDir)
    {
        var virusHand = virusHandScene.Instantiate<VirusHand>();
        var virusWarning = virusWarningScene.Instantiate<VirusWarning>();

        Vector2I screenSize = DisplayServer.ScreenGetSize();
        Vector2I spriteSize = (Vector2I)((RectangleShape2D)virusHand.GetNode<CollisionShape2D>("CollisionShape2D").Shape).Size * 2;

        switch (moveDir)
        {
            case VirusHandMoveDir.LeftRight:
                virusWarning.Init(1, new Vector2(screenSize.X, spriteSize.Y));
                virusWarning.Position = new Vector2I(screenSize.X / 2 + Global.WorldOutsideOffset.X, spriteSize.Y / 2 + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.RightLeft:
                virusWarning.Init(1, new Vector2(screenSize.X, spriteSize.Y));
                virusWarning.Position = new Vector2I(screenSize.X / 2 + Global.WorldOutsideOffset.X, screenSize.Y - spriteSize.Y / 2 + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.TopDown:
                virusWarning.Init(1, new Vector2(spriteSize.X, screenSize.Y));
                virusWarning.Position = new Vector2I(screenSize.X - spriteSize.Y / 2 + Global.WorldOutsideOffset.X, screenSize.Y / 2 + Global.WorldOutsideOffset.Y);
                break;

            case VirusHandMoveDir.DownTop:
                virusWarning.Init(1, new Vector2(spriteSize.Y, screenSize.Y));
                virusWarning.Position = new Vector2I(spriteSize.Y / 2 + Global.WorldOutsideOffset.X, screenSize.Y / 2 + Global.WorldOutsideOffset.Y);
                break;
        }

        virusHand.Init(moveDir);
        virusParent.AddChild(virusWarning);
        virusWarning.SpawnVirus += ActuallySpawnVirusHand;
        virusHands.Push(virusHand);
    }

    private void ActuallySpawnVirusHand()
    {
        virusParent.AddChild(virusHands.Pop());
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
        if (Input.IsKeyPressed(Key.Escape))
        {
            GetTree().Quit();
        }

        if (Input.IsActionJustPressed("skip_conversation"))
        {
            dialogBox.SkipConversation();
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

    private void OnDamageOverTimeTimeout()
    {
        foreach (var child in virusParent.GetChildren())
        {
            if (child is VirusCannon virusCannon)
            {
                virusCannon.HealthComponent.Damage(5);
            }
        }
    }

    private void OnVirusHandSpawnTimerTimeout()
    {
        SpawnVirusHand(virusHandMoveDirs[GD.Randi() % virusHandMoveDirs.Length]);
    }

    private void OnVirusCannonSpawnTimerTimeout()
    {
        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
        SpawnVirusCannon(isUsingSubWindow: false, virusProjectileSpeed: 400);
    }
}
