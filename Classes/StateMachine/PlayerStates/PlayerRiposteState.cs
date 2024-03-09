using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerRiposteState : State
    {
        private const double DefaultTime = 2.3667;
        private Player player;
        private double riposteTimer = DefaultTime;

        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            player ??= Actor as Player;
            player._CanRotate = false;
            player.CurrentWeapon.AddRiposte();
            player.Vulnerable = false;
            riposteTimer = DefaultTime;
            Movement.SetSpeed(0);
            (Animation as PlayerAnimation).MainAttack("Riposte");
            Stam.DecreaseStamina(player.CurrentWeapon.LightAttackStamConsumption);
        }


        public override void PhysicsUpdate(double delta)
        {
            riposteTimer -= delta;
            if (riposteTimer <= 0)
            {
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(PlayerRiposteState));
            }
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player.CurrentWeapon.RestoreDefaultDamage();
            (Animation as PlayerAnimation).CurrentAttack = "";
            (Animation as PlayerAnimation).CurrentMovementState = "Run";
            Movement.SetSpeed(Movement.Speed);
            player.Vulnerable = true;
            player._CanRotate = true;
            riposteTimer = DefaultTime;
        }
    }
}