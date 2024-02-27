using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIChaseState : AIState
    {

        private Vector3 direction = Vector3.Zero;
        private AnimationNodeStateMachinePlayback movementAnim;

        public override void Enter(Player target)
        {
            movementAnim ??= (AnimationNodeStateMachinePlayback)Animation.AnimTree.Get("parameters/Movement/playback");
            
            Animation.Transition("Movement");
            //Animation.AnimTree.Set("parameters/Movement/conditions/Chase", true);
            Animation.NodeTransition("Chase");


            this.target = target;
           
            Movement.SetSpeed(Movement.Speed);
        }

        public override void Exit()
        {
            //Animation.AnimTree.Set("parameters/Movement/conditions/Chase", false);
            target = null;
        }

        public override void PhysicsUpdate(double delta)
        {
            ChaseTarget(delta);
        }

        public override void Update(double delta)
        {
            
        }

        public void ChaseTarget(double delta)
        {   
            if (target == null) return;
            direction = AIActor.GlobalPosition.DirectionTo(target.GlobalPosition);
            direction += AIActor.DisplacementTest();
            
            Animation.AnimTree.Set("parameters/Movement/Chase/blend_position", Vector2.Down);
            AIActor.LookInDirection(direction);

            Movement.HandleMovement(direction, delta);
        }
   }
}
