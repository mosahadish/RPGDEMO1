using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerParryingState : State
    {
        private const double DefaultTimer = 0.5;
        private Player player;
        private string currentTransition;

        private Vector3 newVelo;
        private float timer;
        private Weapon currentOffhand;
        private string offhandType;

        private double parryTimer = DefaultTimer;

        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            currentOffhand = player.CurrentOffhand;
            offhandType = currentOffhand.GetType().Name;

            (Animation as PlayerAnimation).Parry = true;
            (Animation as PlayerAnimation).BlendMovement(Vector2.Zero);
            (Animation as PlayerAnimation).RequestOneShot("Offhand");
            Stam.DecreaseStamina((player.CurrentOffhand as ParryingObject).StaminaConsumption);
            (player.CurrentOffhand as ParryingObject).ActivateParryWindow();

            player.Velocity = Vector3.Zero;
            player._CanRotate = false;
            Movement.SetSpeed(0);
        }

        public override void PhysicsUpdate(double delta)
        {
            newVelo = player.Velocity;
            newVelo.Y += -Movement.Gravity * (float)delta;
            
            player.Velocity = newVelo;
            player.MoveAndSlide();

            parryTimer -= delta;

            if (parryTimer <= 0)
            {
                EmitSignal(SignalName.StateFinishedWithArgument, nameof(PlayerParryingState));
            }
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            player._CanRotate = true;
            parryTimer = DefaultTimer;
            (Animation as PlayerAnimation).Parry = false;
            //GD.Print(currentTransition);
            Animation.Transition(offhandType, currentTransition);
        }
    }
}