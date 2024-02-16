using Godot;
using System.Collections.Generic;
using Globals;
using System;

namespace Game
{
    [GlobalClass]
    public partial class AICircleState : State
    {
        private Actor target = null;

        private Vector2 blendPos = Vector2.Zero;
        private Vector3 dirToTarget;

        private Vector3 circleDir;

        private RandomNumberGenerator rng = new();

        private int[] circleSide = {-1,1};
        private int randIndex = 1;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            Movement.SetSpeed(Movement.WalkSpeed);
            //change later
            randIndex = (int)(rng.Randi() % circleSide.Length);
            (Animation as AnimateAI).Transition("Circle");
        }

    
        public override void PhysicsUpdate(double delta)
        {
            dirToTarget = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
            Actor.LookInDirection(dirToTarget);
            CircleAround(delta);
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            target = null;
            Movement.SetSpeed(Movement.Speed);
        }

        public void SetTarget(Actor target)
        {
            this.target = target;
        }

        private void CircleAround(double delta)
        {
            if (target == null) return;
            circleDir = dirToTarget.Rotated(Vector3.Up * circleSide[randIndex], Mathf.Pi/2);
            blendPos.X = circleSide[randIndex];
            Animation.BlendPosition("Circle", -1*blendPos);
            Movement.HandleMovement(circleDir, delta);
        }
    }
}
