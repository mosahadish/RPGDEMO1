using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIAttackState : AIState
    {
        private Vector3 dirToTarget;
        private RandomNumberGenerator rng = new();
        private float randI = 0;

        public override void Enter(Player target)
        {
            this.target = target;
            randI = rng.RandiRange(0,10);

            if (randI <= 5) AIActor.Attack1();
            else if (randI >= 9) AIActor.ComboAttack();
            else AIActor.Attack2();
        }
      
        public override void PhysicsUpdate(double delta)
        {
            dirToTarget = AIActor.GlobalPosition.DirectionTo(target.GlobalPosition);
            AIActor.LookInDirection(dirToTarget);
            Movement.HandleMovement(dirToTarget, delta);
        }

        public override void Update(double delta)
        {
        }
        
          public override void Exit()
        {
            AIActor.FinishAttacking();
            target = null;
        }
    }
}
