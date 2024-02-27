using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIAttackState : AIState
    {
        private Vector3 dirToTarget;
        private float distToTarget;
        private RandomNumberGenerator rng = new();
        private float randI = 0;
        private AnimationNodeStateMachinePlayback attackAnim;
        private string currentAnim;

        public override void Enter(Player target)
        {
            attackAnim = (AnimationNodeStateMachinePlayback)Animation.AnimTree.Get("parameters/Attack/playback");
            //attackAnim.
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
            distToTarget = AIActor.GlobalPosition.DistanceTo(target.GlobalPosition);
            
            AIActor.LookInDirection(dirToTarget);

            if (distToTarget >= AIActor.AttackRange/2)
                Movement.HandleMovement(dirToTarget, delta);
        }

        public override void Update(double delta)
        {
        }
        
          public override void Exit()
        {
            (AIActor as IAttacker).FinishAttacking();

            //Animation.Transition("Movement");
            //Animation.NodeTransition("Engage");
            target = null;
        }
    }
}
