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

            if (AIActor.HasAimingWeapon())
            {
                if (AIActor is IRangeAttacker range)
                {
                    range.RangeAttack1();
                }
            }

            else 
            {
                if (AIActor is IMeleeAttacker melee)
                if (randI <= 5) melee.Attack1();
                else if (randI >= 9) melee.ComboAttack();
                else melee.Attack2();
            }
            
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
            (AIActor as IAttacker).FinishAttacking();
            target = null;
        }
    }
}
