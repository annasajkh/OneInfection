using Godot;

namespace OneInfection.Src.Windows
{
    public abstract partial class ViewWindow : Window
    {
        [Export] private RemoteTransform2D positionToView;

        [Export] private Camera2D camera;

        public override void _Ready()
        {
            positionToView.RemotePath = camera.GetPath();
        }
    }
}
