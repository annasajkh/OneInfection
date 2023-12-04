using Godot;
using OneInfection.src.Utils;
using OneInfection.Src.NikoScene;
using OneInfection.Src.NikoWindowScene;
using OneInfection.Src.Utils;
using System;

namespace OneInfection.Src.MainScene
{
    public partial class Main : Node2D
    {
        public bool IsMainWindowShaking { get; set; }

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


            dialogBox.Call("play", DialogParser.Parse("assets/dialogs/Goodbye.json"));

            dialogBox.Connect("dialog_finished", new Callable(this, nameof(GoodbyeDialogFinished)));
        }

        private void GoodbyeDialogFinished()
        {
            animationPlayer.Play("goodbye_niko");
            dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(GoodbyeDialogFinished)));
        }

        public void SomethingIsWrongWithTWM()
        {
            dialogBox.Call("play", DialogParser.Parse("assets/dialogs/SomethingIsWrongWithTWM.json"));

            dialogBox.Connect("dialog_finished", new Callable(this, nameof(VirusInfectingTWM)));
        }

        public void VirusInfectingTWM()
        {
            dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(VirusInfectingTWM)));

            IsMainWindowShaking = true;

            dialogBox.Call("play", DialogParser.Parse("assets/dialogs/VirusInfectingTWM.json"));


            dialogBox.Connect("dialog_finished", new Callable(this, nameof(VirusTakingOverTWM)));
        }

        public void VirusTakingOverTWM()
        {
            dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(VirusTakingOverTWM)));

            IsMainWindowShaking = false;

            mainWindow.MoveToCenter();

            dialogBox.Call("play", DialogParser.Parse("assets/dialogs/VirusTakingOverTWM.json"));

            dialogBox.Connect("dialog_finished", new Callable(this, nameof(BattleStart)));
        }

        private void BattleStart()
        {
            dialogBox.Disconnect("dialog_finished", new Callable(this, nameof(BattleStart)));

            // here we starting our fight

            niko.IsControlled = true;

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

            niko.Position = Util.ToWorldPosition(nikoWindowOffset);
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

            if (niko.IsOutside)
            {
                nikoWindow.Position = Util.ToScreenPosition((Vector2I)niko.Position);
            }
        }
    }
}
