using Godot;
using System.Collections.Generic;
using Globals;
using System;

namespace Game
{
    [GlobalClass]
    public partial class AIRoamState : State
    {

        private Vector3 newVelo;
        private double waitTime;
        private double wanderTime;
        private Vector3 wanderDirection = Vector3.Zero;


        private RandomNumberGenerator rng = new();
        public override void Enter(Dictionary<string, Vector2> msg)
        {
            (Animation as AnimateAI).Transition("Roam");
            SetWanderTime();
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void Exit()
        {
        }

        public override void PhysicsUpdate(double delta)
        {
            //gravity
            newVelo = Actor.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            Actor.Velocity = newVelo;
            //gravity
            
            RandomWandering(delta);
            Actor.MoveAndSlide();
        }

        public override void Update(double delta)
        {
        }

        private void RandomWandering(double delta)
        {
            if (wanderTime > 0)
            {   
                if ((Actor as AI).Raycasts.IsOnFloor())
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
                Actor.LookInDirection(wanderDirection);
            }
        }

        private void WanderInDirection(Vector3 direction, double delta)
        {
            Animation.BlendPosition("Roam", Vector2.Down);
            Movement.HandleMovement(direction, delta);
            Actor.LookInDirection(direction);
            wanderTime -= delta;
        }

        private void SetWanderDirection()
        {
            wanderDirection.X = rng.RandfRange(-1, 1);
            wanderDirection.Z = rng.RandfRange(-1, 1);
            wanderDirection.Y = 0;
        }

        private void Wait(double delta)
        {
            Actor.Velocity = Vector3.Zero;
            Animation.BlendPosition("Roam", Vector2.Zero);
            waitTime -= delta;
        }

        private void SetWaitTime()
        {
            waitTime = rng.RandfRange(1,3);
        }

        private void SetWanderTime()
        {
            wanderTime = rng.RandfRange(1,3);
        }
    }
}