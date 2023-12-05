using Godot;
using OneInfection.Src.NikoScene;
using OneInfection.Src.Utils;
using System.Collections.Generic;


namespace OneInfection.Src.DialogBoxScene
{
    public partial class DialogBox : Control
    {
        [Export] private Label dialog;
        [Export] private Timer speakDelay;
        [Export] private TextureRect faceIcon;
        [Export] private AudioStreamPlayer dialogSound;
        [Export] private TextureRect continueDialogArrow;
        [Export] private AnimationPlayer continueDialogArrowAnimation;
        [Export] private Timer acceptTimer;

        [Export] private OutsideDialogBox outsideDialogBox;
        [Export] private Niko niko;

        [Signal] public delegate void ConversationFinishedEventHandler();

        private float charPerSeconds = 60;
        private int currentConversationIndex;
        private bool conversationFinished;
        private bool dialogPaused;

        private List<DialogItem> conversation;

        private AudioStream normalDialogSound;
        private AudioStream robotDialogSound;


        public override void _Ready()
        {
            Visible = false;

            normalDialogSound = GD.Load<AudioStream>("res://assets/sounds/normal_dialog.wav");
            robotDialogSound = GD.Load<AudioStream>("res://assets/sounds/robot_dialog.wav");

            outsideDialogBox.ConversationFinished += NikoDialogFinished;
        }
        public void Play(List<DialogItem> conversation)
        {
            Visible = true;
            currentConversationIndex = 0;
            this.conversation = conversation;

            SetNextDialogBox();
        }

        private void SetNextDialogBox()
        {
            if (currentConversationIndex > conversation.Count - 1)
            {
                speakDelay.Stop();
                Visible = false;
                EmitSignal(SignalName.ConversationFinished);

                return;
            }

            string characterName = conversation[currentConversationIndex].face.Split("/")[0];

            if (characterName == "niko" && niko.IsOutside)
            {
                outsideDialogBox.Play(new List<DialogItem>() { conversation[currentConversationIndex] });
                currentConversationIndex++;

                return;
            }
            else
            {
                Visible = true;
            }

            dialog.VisibleCharacters = 0;

            acceptTimer.WaitTime = conversation[currentConversationIndex].delayToNext;
            dialog.Text = conversation[currentConversationIndex].dialog;
            faceIcon.Texture = GD.Load<Texture2D>($"res://assets/textures/faces/{conversation[currentConversationIndex].face}.png");

            if (characterName == "en" || characterName == "virus")
            {
                dialogSound.Stream = robotDialogSound;
            }
            else
            {
                dialogSound.Stream = normalDialogSound;
            }

            currentConversationIndex++;

            speakDelay.WaitTime = dialog.Text.Length / (dialog.Text.Length * charPerSeconds);
            speakDelay.Start();
        }


        public override void _Process(double delta)
        {
            if (conversationFinished)
            {
                continueDialogArrow.Visible = true;
                continueDialogArrowAnimation.Play("up_and_down");
            }
            else
            {
                continueDialogArrow.Visible = false;
                continueDialogArrowAnimation.Stop();
            }

            if (Input.IsActionJustPressed("ui_accept") && conversationFinished)
            {
                SetNextDialogBox();
                conversationFinished = false;
            }

            if (Input.IsActionJustPressed("ui_accept") && dialogPaused)
            {
                speakDelay.Start();
                dialogPaused = false;
            }
        }

        #region Signal receivers

        private void NikoDialogFinished()
        {
            SetNextDialogBox();
        }

        private void OnSpeakDelayTimeout()
        {
            dialog.VisibleCharacters++;

            if (dialog.VisibleCharacters < dialog.Text.Length - 4 && !dialogSound.Playing)
            {
                dialogSound.Play();
            }

            if (dialog.VisibleCharacters == dialog.Text.Length)
            {
                speakDelay.Stop();
                acceptTimer.Start();
            }
        }

        private void OnAcceptTimerTimeout()
        {
            conversationFinished = true;
        }

        #endregion
    }

}