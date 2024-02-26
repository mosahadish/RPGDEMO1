using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Shield : ParryingObject
    {
        public override void _Ready()
        {
            base._Ready();
            _CanBlock = true;
        }
    }
}
