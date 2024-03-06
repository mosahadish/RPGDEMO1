using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerIdleState : State
    {

        private Player player;
        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            player??= Actor as Player;
        }


        public override void PhysicsUpdate(double delta)
        {
            if (player._InAir) return;
            if (player.IsAttacking()) return;
            if (player.IsDodging()) return;
        
            Stam.IncreaseStamina(Stam.RegenRate);
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
           
        }
    }
}