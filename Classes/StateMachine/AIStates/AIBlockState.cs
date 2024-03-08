using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIBlockState : AIState
    {
        private const double DefaultTimer = 0.6; //For counter attack flag
        private double counterTimer = DefaultTimer;
        private double blockTimer;
        private RandomNumberGenerator rng = new();

        public override void Enter(Player target)
        {
            this.target = target;
            if (AIActor is IBlocker blocker)
            {
                blocker.Block();
            } 

            blockTimer = rng.RandiRange(1,5);

            Animation.CurrentMovementState = "Walk";
            Animation.Blocking = true;
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void PhysicsUpdate(double delta)
        {
            blockTimer -= delta;
            if (blockTimer <= 0)
            {
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(AIBlockState));
            }

            if (AIActor is IBlocker blocker)
            {
                if (blocker.CanCounter())
                {
                    counterTimer -= delta;
                    if (counterTimer <= 0)
                    {
                        blocker.ResetBlockedAttack();
                        counterTimer = DefaultTimer;
                    }
                }
            }
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            if (AIActor is IBlocker blocker)
            {
                blocker.BlockRelease();
            }
            Animation.Blocking = false;
            target = null;
        }

   
    }
}