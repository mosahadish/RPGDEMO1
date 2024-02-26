using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerParryingState : State
    {
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;
        private float timer;
        private Weapon currentOffhand;
        private string offhandType;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            currentOffhand = player.CurrentOffhand;
            offhandType = currentOffhand.GetType().Name;

            currentTransition = (string)Animation.AnimTree.Get("parameters/"+ offhandType +"/current_state");
            Animation.Transition(offhandType, "Parry");

            (player.CurrentOffhand as ParryingObject).ActivateParryWindow();

            player.Velocity = Vector3.Zero;
            player._CanRotate = false;
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            newVelo = player.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            
            player.Velocity = newVelo;
            player.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player._CanRotate = true;
            GD.Print(currentTransition);
            Animation.Transition(offhandType, currentTransition);
        }
    }
}