using Godot;
using OneInfection.Src.DialogBoxScenes.DialogBoxScene;
using System.Collections.Generic;

namespace OneInfection.Src.DialogBoxScenes.OutsideDialogBoxScene;



public partial class OutsideDialogBox : Node2D
{
    [Export] private Label dialog;
    [Export] private Timer speakDelay;
    [Export] private AudioStreamPlayer dialogSound;
    [Export] private Timer acceptTimer;


    [Signal] public delegate void ConversationFinishedEventHandler();

    private float charPerSeconds = 60;
    private int currentConversationIndex;
    private bool conversationFinished;
    private bool dialogPaused;
    private bool isAutoPlay;

    private List<DialogItem> conversation;

    public override void _Ready()
    {
        Visible = false;
        dialogSound.Stream = GD.Load<AudioStream>("res://assets/sounds/normal_dialog.wav");
    }

    public void Play(List<DialogItem> conversation, bool isAutoPlay = false)
    {
        this.isAutoPlay = isAutoPlay;
        this.conversation = conversation;

        currentConversationIndex = 0;
        Visible = true;

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

        dialog.VisibleCharacters = 0;

        acceptTimer.WaitTime = conversation[currentConversationIndex].delayToNext;
        dialog.Text = conversation[currentConversationIndex].dialog;


        currentConversationIndex++;

        speakDelay.WaitTime = dialog.Text.Length / (dialog.Text.Length * charPerSeconds);
        speakDelay.Start();
    }


    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept") && conversationFinished && !isAutoPlay)
        {
            SetNextDialogBox();
            conversationFinished = false;
        }

        if (Input.IsActionJustPressed("ui_accept") && dialogPaused && !isAutoPlay)
        {
            speakDelay.Start();
            dialogPaused = false;
        }
    }


    #region Signal receivers
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

        if (isAutoPlay)
        {
            SetNextDialogBox();
            conversationFinished = false;
        }
    }
    #endregion
}
