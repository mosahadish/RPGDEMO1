using System.Collections.Generic;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Attack : Node
    {

        [ExportCategory("AttackStats")]
        [Export] public float Damage;
        
        [ExportCategory("Dependencies")]
        [Export] private StateMachine sMachine;
        [Export] public Actor Actor;

        public Weapon CurrentWeapon; //Set by the StateMachine

        [Export] public bool ReadyToShoot = false;
        public Projectile Proj;

        public void WantsLightAttack(bool pressed)
        {
            Dictionary<string, bool> msg = new()
            {
                { Actions.LightAttack, pressed }
            };
            sMachine.HandleAttackInput(msg);
        }

        public void WantsBlock(bool pressed)
        {
            Dictionary<string, bool> msg = new()
            {
                { Actions.Block, pressed }
            };
            sMachine.HandleAttackInput(msg);
        }

        public void WantsParry(bool pressed)
        {
            Dictionary<string, bool> msg = new()
            {
                { Actions.Parry, pressed }
            };
            sMachine.HandleAttackInput(msg);
        }
    }
}
