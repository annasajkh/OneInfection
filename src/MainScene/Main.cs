using Godot;
using OneInfection.Src.DialogBoxScenes.DialogBoxScene;
using OneInfection.Src.NikoScenes.NikoScene;
using OneInfection.Src.NikoScenes.NikoWindowScene;
using OneInfection.Src.Utils;
using System;

namespace OneInfection.Src.MainScene
{
    public partial class Main : Node2D
    {
        [Export] private Node2D subWindows;
        [Export] private Niko niko;
        [Export] private NikoWindow nikoWindow;
        [Export] private DialogBox dialogBox;
        [Export] private AnimationPlayer animationPlayer;

        public bool IsMainWindowShaking { get; set; }

        private Window mainWindow;
        private Vector2I windowPosition;
        private bool forceCenter = true;

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

            mainWindow.MoveToCenter();

            dialogBox.Play(DialogParser.Parse("assets/dialogs/VirusTakingOverTWM.json"));

            dialogBox.ConversationFinished += BattleStart;
        }

        private void BattleStart()
        {
            dialogBox.ConversationFinished -= BattleStart;



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

            if (niko.IsOutside)
            {
                nikoWindow.Position = Util.ToScreenPosition((Vector2I)niko.Position);
            }
        }
    }
}
