using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerStaggerState : State
    {
        private Player player;
        private string currentTransition;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            currentTransition = (string)Animation.AnimTree.Get("parameters/TypeTransition/current_state");
            Animation.Transition("TypeTransition", "STAGGER");
            player.Velocity = Vector3.Zero;
            Actor._CanRotate = false;
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            player.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            Actor._CanRotate = true;
            Animation.Transition("TypeTransition", currentTransition);
        }
    }
}