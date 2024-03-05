using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerPickUpState : State
    {
        private const double DefaultTimer = 0.8;
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;

        private double pickUpTimer = DefaultTimer;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            player.Velocity = Vector3.Zero;
            player._CanRotate = false;

            (Animation as PlayerAnimation).Pickup = true;
			(Animation as PlayerAnimation).RequestOneShot("Interact");
            
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            pickUpTimer -= delta;
            if (pickUpTimer <= 0)
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(PlayerPickUpState));
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            pickUpTimer = DefaultTimer;
            player._CanRotate = true;
            (Animation as PlayerAnimation).Pickup = false;
            
            Movement.SetSpeed(Movement.Speed);
        }
    }
}