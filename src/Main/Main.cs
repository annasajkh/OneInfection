using Godot;
using Godot.Collections;
using OneInfection.src.Utils;
using System;

public partial class Main : Node2D
{
    [Export]
    private Node2D subWindows;

    [Export]
    private Niko niko;

    [Export]
    private NikoWindow nikoWindow;

    [Export]
    private Control dialogBox;

    [Export]
    private AnimationPlayer animationPlayer;

    [Export]
    private Timer windowShakeTimer;

    private Window mainWindow;

    private Vector2I windowPosition;

    public override void _Ready()
    {
        mainWindow = GetWindow();

        foreach (var child in subWindows.GetChildren())
        {
            if (child is Window window)
            {
                window.World2D = mainWindow.World2D;
            }
            else
            {
                throw new Exception("All child of SubWindows should be type of Window");
            }
        }

        dialogBox.Call("play", new Array<Array<Variant>>()
        {
            new()
            {
                "niko/niko2", "Goodbye, Player...", 0.5f
            }
        });

        dialogBox.Connect("dialog_finished", new Callable(this, nameof(GoodbyeDialogFinished)));
    }

    private void GoodbyeDialogFinished()
    {
        animationPlayer.Play("goodbye_niko");
        dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(GoodbyeDialogFinished)));
    }

    public void SomethingIsWrongWithTWM()
    {
        dialogBox.Call("play", new Array<Array<Variant>>()
        {
            new()
            {
                "en/en_distressed_meow", "Wait Niko...", 2
            },

            new()
            {
                "niko/niko", "The World Machine? what happened?", 0.1f
            },

            new()
            {
                "en/en_distressed_meow", "There....", 0.1f
            },

            new()
            {
                "en/en_distressed_talk", "Is....", 0.1f
            },

            new()
            {
                "en/en_distressed_meow", "Something....", 0.1f
            },

            new()
            {
                "en/en_distressed_talk", "Wrong....", 0.1f
            },

            new()
            {
                "en/en_distressed_meow", "With....", 0.1f
            },

            new()
            {
                "en/en_distressed_talk", "Me....", 0.1f
            },
        });

        dialogBox.Connect("dialog_finished", new Callable(this, nameof(VirusInfectingTWM)));
    }

    public void VirusInfectingTWM()
    {
        dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(VirusInfectingTWM)));

        windowPosition = mainWindow.Position;

        windowShakeTimer.Start();
        dialogBox.Call("play", new Array<Array<Variant>>()
        {
            new()
            {
                "en/en_shock", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 0.1f
            },

            new()
            {
                "virus/virus_shock", "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", 0.1f
            },
        });

        dialogBox.Connect("dialog_finished", new Callable(this, nameof(VirusTakingOverTWMDialog)));
    }

    public void VirusTakingOverTWMDialog()
    {
        dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(VirusTakingOverTWMDialog)));

        windowShakeTimer.Stop();
        mainWindow.Position = windowPosition;

        dialogBox.Call("play", new Array<Array<Variant>>()
        {
            new()
            {
                "virus/virus_83c", "Well hello there", 0.1f
            },

            new()
            {
                "niko/niko", "The World Machine is everything alright? what just happened?", 0.1f
            },

            new()
            {
                "virus/virus5", "Oh? The World Machine?", 0.1f
            },

            new()
            {
                "virus/virus_83c", "so you call them \"The World Machine\" very interesting", 0.1f
            },

            new()
            {
                "niko/niko", "Who are you? what did you do to The World Machine?", 0.1f
            },

            new()
            {
                "virus/virus_smile", "I'm a virus and i just infected The World Machine", 0.1f
            },

            new()
            {
                "virus/virus_83c", "so this is why you call them \"The World Machine\" because there is a little simulation in here", 0.1f
            },

            new()
            {
                "niko/niko", "don't you dare do anything with the simulation", 0.1f
            },

            new()
            {
                "virus/virus2", "Ugh i can't! it seems like there some sort of protection in here", 0.1f
            },

            new()
            {
                "virus/virus_smile", "Is it you? are you the protection?", 0.1f
            },

            new()
            {
                "virus/virus_83c", "well let me find that out by infecting you", 0.1f
            }
        });

		dialogBox.Connect("dialog_finished", new Callable(this, nameof(BattleStart)));
	}

    private void BattleStart()
    {
		dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(BattleStart)));

        // here we starting our fight


	}

    private void OnWindowShakeTimerTimeout()
    {
        mainWindow.Position += new Vector2I(GD.RandRange(-20, 20), GD.RandRange(-20, 20));
    }

    private void OnFirstHouseGoOutside()
    {
        niko.Speed *= 2;

        Vector2I initialWindowPosition = DisplayServer.WindowGetPosition();
        Vector2I initialWindowSize = DisplayServer.WindowGetSize();

        Vector2I nikoWindowOffset = initialWindowPosition;

        nikoWindowOffset.X += initialWindowSize.X / 2 - nikoWindow.Size.X / 2 + 10;
        nikoWindowOffset.Y += initialWindowSize.Y - nikoWindow.Size.Y / 2 - 32;

        niko.IsBright = false;
        nikoWindow.Visible = true;
        niko.IsOutside = true;

        niko.Position = Utils.ToWorldPosition(nikoWindowOffset);
    }

    public override void _Process(double delta)
    {

        if (niko.IsOutside)
        {
            nikoWindow.Position = Utils.ToScreenPosition((Vector2I)niko.Position);
        }
    }
}

// this comment leaves hope
