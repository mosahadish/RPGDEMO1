using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerDrinkState : State
    {
        private const double defaultDrinkTime = 2;
        private Player player;
        private Vector3 newVelo;

        private Weapon weaponToRedraw = null;

        private double drinkTimer = defaultDrinkTime;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;

            (Animation as PlayerAnimation).CurrentMovementState = "Walk";
           
            if (player.HasOffhand()) weaponToRedraw = player.CurrentOffhand;
            else if (player.HasWeapon() && player.CurrentWeapon is Bow) weaponToRedraw = player.CurrentWeapon;

            player.Equip.SheatheWeapon(weaponToRedraw);

            (Animation as PlayerAnimation).UseItem = "Drink";
			(Animation as PlayerAnimation).RequestOneShot("UseItem");
            
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void PhysicsUpdate(double delta)
        {
            drinkTimer -= delta;
            if (drinkTimer <= 0) 
            {
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(PlayerDrinkState));
            }
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            if (weaponToRedraw != null) player.Equip.DrawWeapon(weaponToRedraw);
            (Animation as PlayerAnimation).CurrentMovementState = "Run";
            weaponToRedraw = null;
            (Animation as PlayerAnimation).UseItem = "";
            player.Consuming = false;
            Movement.SetSpeed(Movement.Speed);
            drinkTimer = defaultDrinkTime;
        }
    }
}