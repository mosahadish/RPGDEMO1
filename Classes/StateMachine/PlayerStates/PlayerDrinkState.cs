using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerDrinkState : State
    {
        private Player player;
        private Vector3 newVelo;

        private Weapon weaponToRedraw = null;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            player.Velocity = Vector3.Zero;
            player._CanRotate = false;

            //(Animation as PlayerAnimation).CurrentMovementState = "Walk";
            (Animation as PlayerAnimation).BlendMovement(Vector2.Zero);
            if (player.HasOffhand()) weaponToRedraw = player.CurrentOffhand;
            player.Equip.SheatheWeapon(weaponToRedraw);

            (Animation as PlayerAnimation).UseItem = "Drink";
			(Animation as PlayerAnimation).RequestOneShot("UseItem");
            
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            newVelo = player.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            
            player.Velocity = newVelo;
            player.MoveAndSlide();
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player._CanRotate = true;
            if (weaponToRedraw != null) player.Equip.DrawWeapon(weaponToRedraw);
            //(Animation as PlayerAnimation).CurrentMovementState = "Run";
            weaponToRedraw = null;
            (Animation as PlayerAnimation).UseItem = "";
            
            Movement.SetSpeed(Movement.Speed);
        }
    }
}