using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIAirState : State
    {
        private Vector2 inputDir;
        private Vector3 newVelo;


        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            if (Msg.ContainsKey(Actions.Jump))
            {
                if (Msg[Actions.Jump] != Vector2.Zero) Anim = Animations.JumpRunning;
                else Anim = Animations.JumpStand;

                newVelo = Actor.Velocity;
                newVelo.Y = Movement.JumpVelocity;
                Actor.Velocity = newVelo;
            
                SetAnim(Animations.TransitionMovement, Anim);
                Animation.Transition(AnimTransition, Anim);
            }

            Actor._CanRotate = false;
            Actor._InAir = true;
        }


        public override void PhysicsUpdate(double delta)
        {
            newVelo = Actor.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;

            Actor.Velocity = newVelo;
            Actor.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            Actor._InAir = false;
            Actor._CanRotate = true;
        }
    }
}