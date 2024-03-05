using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerBlockState : State
    {

        private const double DefaultTimer = 0.6; //For counter attack flag
        private Player player;
        private double counterTimer = DefaultTimer;

        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            player ??= Actor as Player;

            (Animation as PlayerAnimation).CurrentMovementState = "Walk";
			(Animation as PlayerAnimation).Blocking = true;
            Movement.SetSpeed(Movement.WalkSpeed);
            counterTimer = DefaultTimer;
            player.Block();
        }


        public override void PhysicsUpdate(double delta)
        {
            Stam.DecreaseStamina(Stam.DegenRate);
            
            if (player.CanCounter())
            {
                counterTimer-= delta;
                if (counterTimer <= 0)
                {
                    player.ResetBlockedAttack();
                    counterTimer = DefaultTimer;
                }
            }
            
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
           (Animation as PlayerAnimation).Blocking = false;
           (Animation as PlayerAnimation).CurrentMovementState = "Run";
           Movement.SetSpeed(Movement.Speed);
           player.BlockRelease();
        }
    }
}