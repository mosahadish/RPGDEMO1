using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerRestingState : State
    {
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;

        private AnimationNodeStateMachinePlayback t;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;

            player.Velocity = Vector3.Zero;
            player._CanRotate = false;
            (Animation as PlayerAnimation).Resting = true;

            t = (AnimationNodeStateMachinePlayback)(Animation as PlayerAnimation).AnimTree.Get("parameters/InteractState/playback");
            GD.Print(t.GetCurrentNode());
            
            (Animation as PlayerAnimation).RequestOneShot("Interact");
            
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
   
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player._CanRotate = true;
            
            //(Animation as PlayerAnimation).CurrentMovementState = "Run";
            Movement.SetSpeed(Movement.Speed);
        }
    }
}