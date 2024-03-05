using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAirState : State
    {
        private Vector2 inputDir;
        private Vector3 newVelo;

        private double airTime = 0;

        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            if (Msg.ContainsKey(Actions.Jump))
            {
                if (Msg[Actions.Jump] != Vector2.Zero) (Animation as PlayerAnimation).JumpType = "Running";
                else (Animation as PlayerAnimation).JumpType = "Standing";

                newVelo = Actor.Velocity;
                newVelo.Y = Movement.JumpVelocity;
                Actor.Velocity = newVelo;
                (Animation as PlayerAnimation).RequestOneShot("Air");
            }
            
            // (Animation as PlayerAnimation).Fall = true;

            // (Animation as PlayerAnimation).FallState = "Light";
            
            Actor._CanRotate = false;
            Actor._InAir = true;
        }


        public override void PhysicsUpdate(double delta)
        {
            newVelo = Actor.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            airTime += delta;

            Actor.Velocity = newVelo;
            Actor.MoveAndSlide();
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            GD.Print(airTime);
            // if (airTime >= 2)
            // {
            //     airTime *= 5;
            //     Actor.HP.TakeDamage((float)airTime);
            // }
            // (Animation as PlayerAnimation).Fall = false;
            // (Animation as PlayerAnimation).LandState = "Roll";
            Actor._InAir = false;
            Actor._CanRotate = true;
            airTime = 0;
        }
    }
}