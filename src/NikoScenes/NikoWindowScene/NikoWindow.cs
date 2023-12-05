using Godot;
using OneInfection.Src.NikoScenes.NikoScene;


namespace OneInfection.Src.NikoScenes.NikoWindowScene
{

    public partial class NikoWindow : Window
    {
        public bool IsOutside;

        [Export] private RemoteTransform2D positionToView;

        [Export] private Camera2D camera;

        [Export] private Niko niko;


        private float speed = 120;
        public override void _Ready()
        {
            positionToView.RemotePath = camera.GetPath();
        }
    }
}
