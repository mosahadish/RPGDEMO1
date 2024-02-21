using Godot;
using System.Collections.Generic;
using Globals;
using System;

namespace Game
{
    [GlobalClass]
    public partial class AIEngageState : AIState
    {
        private Vector2 blendPos = Vector2.Zero;
        private Vector3 dirToTarget;
        private float distToTarget;
    

        private Vector3 circleDir = Vector3.Zero;

        private RandomNumberGenerator rng = new();

        private int[] circleSide = {-1,1};
        private int randIndex = 1;

        public override void Enter(Player target)
        {
            this.target = target;
            Animation.Transition("Engage");
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void PhysicsUpdate(double delta)
        {
            distToTarget = AIActor.GlobalPosition.DistanceTo(target.GlobalPosition);
            dirToTarget = AIActor.GlobalPosition.DirectionTo(target.GlobalPosition);
            AIActor.LookInDirection(dirToTarget);
          
            
            if (distToTarget < AIActor.AttackRange*0.8)
            {
                dirToTarget = -dirToTarget;
                Movement.HandleMovement(dirToTarget, delta);
                //blendPos.X = 0;
                //blendPos.Y = dirToTarget.Z;
                //Animation.BlendPosition("Engage", -1*blendPos);
            }

            else if (distToTarget > AIActor.AttackRange)
            {
                Movement.HandleMovement(dirToTarget, delta);
                //blendPos.X = 0;
                //blendPos.Y = dirToTarget.Z;
                //Animation.BlendPosition("Engage", -1*blendPos);
            }

            CircleAround(delta);
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            Movement.SetSpeed(Movement.Speed);
            target = null;
        }

        private void CircleAround(double delta)
        {
            if (target == null) return;
            
            circleDir = dirToTarget.Rotated(Vector3.Up * circleSide[randIndex], Mathf.Pi/2);
            blendPos.X = circleSide[randIndex];
            blendPos.Y = 0;
            Animation.BlendPosition("Engage", -1*blendPos);
            Movement.HandleMovement(circleDir, delta);
        }
    }
}
