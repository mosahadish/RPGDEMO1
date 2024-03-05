using Godot;
using System.Collections.Generic;

namespace Game
{
    [GlobalClass]
    public partial class PlayerSprintState : State
    {
        private Player player;

        public override void Enter(Dictionary<string, Vector2> Msg)
        {   
            player ??= Actor as Player;

            (Animation as PlayerAnimation).CurrentMovementState = "Sprint";
            Movement.SetSpeed(Movement.SprintSpeed);
            Movement._Sprinting = true;
        }


        public override void PhysicsUpdate(double delta)
        {
            Stam.DecreaseStamina(Stam.DegenRate);
        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            (Animation as PlayerAnimation).CurrentMovementState = "Run";
            Movement.SetSpeed(Movement.Speed);
            Movement._Sprinting = false;
        }
    }
}