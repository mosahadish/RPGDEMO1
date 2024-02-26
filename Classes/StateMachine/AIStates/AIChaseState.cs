using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIChaseState : AIState
    {

        private Vector3 direction = Vector3.Zero;

        public override void Enter(Player target)
        {
            this.target = target;
            Animation.Transition("Chase");
            Movement.SetSpeed(Movement.Speed);
        }

        public override void Exit()
        {
            target = null;
        }

        public override void PhysicsUpdate(double delta)
        {
            ChaseTarget(delta);
        }

        public override void Update(double delta)
        {
            
        }

        public void ChaseTarget(double delta)
        {   
            if (target == null) return;
            direction = AIActor.GlobalPosition.DirectionTo(target.GlobalPosition);
            direction += AIActor.DisplacementTest();
            
            Animation.BlendPosition("Chase", Vector2.Down); 
            AIActor.LookInDirection(direction);

            Movement.HandleMovement(direction, delta);
        }
   }
}
