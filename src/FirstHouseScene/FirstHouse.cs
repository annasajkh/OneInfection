using Godot;

namespace OneInfection.Src.FirstHouseScene
{
    public partial class FirstHouse : Node2D
    {
        [Signal] public delegate void GoOutsideEventHandler();

        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {

        }

        private void OnGoOutsideBodyEntered(Node2D body)
        {
            if (body.Name == "Niko")
            {
                EmitSignal(SignalName.GoOutside);
            }
        }
    }
}
