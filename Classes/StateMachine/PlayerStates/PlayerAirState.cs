using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAirState : State
    {
        const double takeDamgeTime = 2;
        private Vector2 inputDir;
        private Vector3 newVelo;

        private double airTime = 0;
        private bool sprint = false;

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

            if (Msg.ContainsKey(Actions.Sprint))
            {
                sprint = true;
                Movement.SetSpeed(Movement.SprintSpeed);
            }            
            else sprint = false;
            // (Animation as PlayerAnimation).Fall = true;

            // (Animation as PlayerAnimation).FallState = "Light";
            
            Actor._CanRotate = false;
            Actor._InAir = true;
        }


        public override void PhysicsUpdate(double delta)
        {
            if (sprint) Stam.DecreaseStamina(Stam.DegenRate);

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
            if (sprint) Movement.SetSpeed(Movement.Speed);
            Actor._InAir = false;
            Actor._CanRotate = true;

            if (airTime >= takeDamgeTime)
            {
                Actor.HP.TakeDamage((float)airTime*6);
            }

            airTime = 0;
        }
    }
}