using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerChangeWeaponState : State
    {
        private Player player;

        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            player ??= Actor as Player;
            // GD.Print((Animation as PlayerAnimation).CurrentWeaponState);
            // GD.Print((Animation as PlayerAnimation).Sheathe);
            // GD.Print((Animation as PlayerAnimation).Unsheathe);
            (Animation as PlayerAnimation).RequestOneShot("ChangeWeapon");
        }


        public override void PhysicsUpdate(double delta)
        {
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            (Animation as PlayerAnimation).Unsheathe = false;
            (Animation as PlayerAnimation).Sheathe = false;
        }
    }
}