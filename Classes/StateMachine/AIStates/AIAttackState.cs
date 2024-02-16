using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIAttackState : State
    {
        private Actor target = null;
        private Vector3 dirToTarget;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            if (msg != null)
            {
                if (msg.ContainsKey(Animations.Attack1)) (Actor as AI).Attack1();
                if (msg.ContainsKey(Animations.Attack2)) (Actor as AI).Attack2();
                if (msg.ContainsKey(Animations.Attack3)) (Actor as AI).Attack3();
            }
        }
      
        public override void PhysicsUpdate(double delta)
        {
            dirToTarget = Actor.GlobalPosition.DirectionTo(target.GlobalPosition);
            Movement.HandleMovement(dirToTarget, delta);
        }

        public override void Update(double delta)
        {
        }
        
          public override void Exit()
        {
            (Actor as AI).FinishAttacking();
            target = null;
        }

        public void SetTarget(Actor target)
        {
            this.target = target;
        }
    }
}
