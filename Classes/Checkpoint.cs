using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Checkpoint : InteractableObject
    {
        [Signal] public delegate void OnCheckPointVisitedWithArgumentEventHandler(Checkpoint cp);

        public bool Visiting = false;
        public bool enemyPresent = false;

        public override void OnInteract()
        {
            if (Visiting == true || enemyPresent) return;

            EmitSignal(SignalName.OnCheckPointVisitedWithArgument, this);
        }

        private void OnEnemyEnteredArea(Node3D body)
        {
            if (body is AI)
            {
                enemyPresent = true;
            }
        }

        private void OnEnemyLeftArea(Node3D body)
        {
            if (body is AI)
            {
                enemyPresent = false;
            }
        }
    }
}