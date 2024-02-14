using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIChaseState : State
    {

        private Vector3 direction = Vector3.Zero;
        private Actor target = null;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            (Animation as AnimateAI).Transition("Chase");
            Movement.SetSpeed(Movement.Speed);

            target = (Actor as AI).Target;
        }

        public override void Exit()
        {
            target = null;
        }

        public override void PhysicsUpdate(double delta)
        {
            ChaseTarget(delta);
            GD.Print("Chasing");
        }

        public override void Update(double delta)
        {
            
        }

        public void ChaseTarget(double delta)
        {
            if (target == null) return;
            direction = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
            
            Animation.BlendPosition("Chase", Vector2.Down); 
            Actor.LookInDirection(direction);

            Movement.HandleMovement(direction, delta);
        }
    }
}
