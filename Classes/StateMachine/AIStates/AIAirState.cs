using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIAirState : AIState
    {
        private Vector2 inputDir;
        private Vector3 newVelo;


        public override void Enter(Player target)
        {   

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
            AIActor._InAir = false;
            AIActor._CanRotate = true;
        }
    }
}