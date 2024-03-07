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
        private double circleTimer;
        private float waitTimer;
        private int randIndex = 1;

        public override void Enter(Player target)
        {
            // mation.Transition("Movement");
            //Animation.AnimTree.Set("parameters/Movement/conditions/Engage", true);
            // Animation.NodeTransition("Engage");
            Animation.CurrentMovementState = "Walk";

            this.target = target;
            distToTarget = 9999;
            Movement.SetSpeed(Movement.WalkSpeed);

            circleTimer = rng.RandfRange(1,3);
            randIndex = rng.RandiRange(0,1);
        }

        public override void PhysicsUpdate(double delta)
        {
            circleTimer -= delta;
            if (circleTimer <= 0)
            {
                randIndex = rng.RandiRange(0,1);
                circleTimer = rng.RandfRange(1,3);
            }

            if (target != null)
            {
                distToTarget = AIActor.GlobalPosition.DistanceTo(target.GlobalPosition);
                dirToTarget = AIActor.GlobalPosition.DirectionTo(target.GlobalPosition);
                AIActor.LookInDirection(dirToTarget);
            }
            else distToTarget = 9999;
          
            
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
            //Animation.AnimTree.Set("parameters/Movement/conditions/Engage", false);
            Movement.SetSpeed(Movement.Speed);
            target = null;
        }

        private void CircleAround(double delta)
        {
            if (target == null) return;
            
            circleDir = dirToTarget.Rotated(Vector3.Up * circleSide[randIndex], Mathf.Pi/2);
            blendPos.X = circleSide[randIndex];
            blendPos.Y = 0;
            // Animation.BlendPosition("Engage", -1*blendPos);
            //Animation.AnimTree.Set("parameters/Movement/Engage/blend_position", -1*blendPos);
            //Animation.BlendPosition("", -1*blendPos);
            Animation.BlendMovement(blendPos);
            Movement.HandleMovement(circleDir, delta);
        }
    }
}
