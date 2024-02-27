using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIStaggerState : AIState
    {
        private Vector3 newVelo;

        public override void Enter(Player target)
        {   
            Movement.SetSpeed(0);
            AIActor._CanRotate = false;
            AIActor.Velocity = Vector3.Zero;
            Animation.Transition("Stagger");
            AIActor.WeaponDamageOff();
        }

        public override void PhysicsUpdate(double delta)
        {
            newVelo = AIActor.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;

            AIActor.Velocity = newVelo;
            AIActor.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            Animation.Transition("Movement");
            AIActor._CanRotate = true;
        }
    }
}