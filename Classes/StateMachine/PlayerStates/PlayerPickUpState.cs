using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerPickUpState : State
    {
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;

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
            // newVelo = player.Velocity;
            // newVelo.Y += -Movement.Gravity * (float)delta;
            
            // player.Velocity = newVelo;
            // player.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player._CanRotate = true;
            (Animation as PlayerAnimation).Pickup = false;
            
            Movement.SetSpeed(Movement.Speed);
        }
    }
}