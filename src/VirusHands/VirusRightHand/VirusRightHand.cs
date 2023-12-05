using Godot;

namespace OneInfection.Src.VirusHands.VirusRightHand
{
    public partial class VirusRightHand : Window
    {
        [Export] private RemoteTransform2D positionToView;

        [Export] private Camera2D camera;

        public override void _Ready()
        {
            positionToView.RemotePath = camera.GetPath();

        }
    }
}