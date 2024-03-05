using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerStaggerState : State
    {
        private const double DefaultTimer = 0.9;
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;
        private double staggerTimer = DefaultTimer;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            //currentTransition = (string)Animation.AnimTree.Get("parameters/TypeTransition/current_state");
            //Animation.Transition("TypeTransition", "STAGGER");
            (Animation as PlayerAnimation).RequestOneShot("Stagger");
            
            player.Velocity = Vector3.Zero;
            Actor._CanRotate = false;
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            newVelo = Actor.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            
            Actor.Velocity = newVelo;
            player.MoveAndSlide();

            staggerTimer -= delta;
            if (staggerTimer <= 0)
            {
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(PlayerStaggerState));
            }
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            Actor._CanRotate = true;
            staggerTimer = DefaultTimer;
            //Animation.Transition("TypeTransition", currentTransition);
        }
    }
}