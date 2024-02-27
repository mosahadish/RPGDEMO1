using Godot;
using System.Collections.Generic;
using Globals;
using System;

namespace Game
{
    [GlobalClass]
    public partial class AIRoamState : AIState
    {
        AnimationNodeStateMachinePlayback movementAnim;

        private Vector3 newVelo;
        private double waitTime;
        private double wanderTime;
        private Vector3 wanderDirection = Vector3.Zero;

        private RandomNumberGenerator rng = new();
        public override void Enter(Player target)
        {
            movementAnim ??= (AnimationNodeStateMachinePlayback)Animation.AnimTree.Get("parameters/Movement/playback");
            
            Animation.Transition("Movement");
            //Animation.AnimTree.Set("parameters/Movement/conditions/Roam", true);
            Animation.NodeTransition("Roam");
            
            SetWanderTime();
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void Exit()
        {
            //Animation.AnimTree.Set("parameters/Movement/conditions/Roam", false);
        }

        public override void PhysicsUpdate(double delta)
        {
            RandomWandering(delta);
        }

        public override void Update(double delta)
        {
        }

        private void RandomWandering(double delta)
        {
            if (wanderTime > 0)
            {   
                if (AIActor.Raycasts.IsOnFloor())
                    WanderInDirection(wanderDirection, delta);
                else
                {
                    wanderTime = 0;
                    SetWaitTime();
                }
            }
            else
            {
                SetWanderDirection();
                Wait(delta);
            }

            if (waitTime < 0)
            {
                SetWaitTime();
                SetWanderTime();
                AIActor.LookInDirection(wanderDirection);
            }
        }

        private void WanderInDirection(Vector3 direction, double delta)
        {
            Animation.BlendPosition("", Vector2.Down);
            direction += AIActor.DisplacementTest();
            direction = direction.Normalized();
            Movement.HandleMovement(direction, delta);
            AIActor.LookInDirection(direction);
            wanderTime -= delta;
        }

        private void SetWanderDirection()
        {
            wanderDirection.X = rng.RandfRange(-1, 1);
            wanderDirection.Z = rng.RandfRange(-1, 1);
            wanderDirection.Y = 0;

            wanderDirection += AIActor.DisplacementTest();
        }

        private void Wait(double delta)
        {
            AIActor.Velocity = Vector3.Zero;
            Animation.BlendPosition("", Vector2.Zero);
            waitTime -= delta;
            AIActor.MoveAndSlide();
        }

        private void SetWaitTime()
        {
            waitTime = rng.RandfRange(2,4);
        }

        private void SetWanderTime()
        {
            wanderTime = rng.RandfRange(3,5);
        }
    }
}