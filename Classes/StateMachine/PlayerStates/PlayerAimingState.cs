using System.Collections.Generic;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAimingState : State
    {
        private const double bowDrawTimer = 1;

        private Player player;

        private double aimTimer;

        AnimationNodeStateMachinePlayback p;
        
        public async override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            (Animation as PlayerAnimation).CurrentMovementState = "Walk";
            (Animation as PlayerAnimation).Aiming = true;
            (Animation as PlayerAnimation).RangeAttack("Draw");

            p = (AnimationNodeStateMachinePlayback)(Animation as PlayerAnimation).AnimTree.Get("parameters/RangeState/playback");

            player.Camera._AimOn = true;
			Movement.SetSpeed(Movement.WalkSpeed);

            if (player.CurrentWeapon is Bow bow)
            {
                aimTimer = bowDrawTimer;
                await ToSignal(GetTree().CreateTimer(0.6), SceneTreeTimer.SignalName.Timeout);
                bow.Draw(); //Bowstring animation
            }
        }

        public override void PhysicsUpdate(double delta)
        {
            GD.Print((Animation as PlayerAnimation).ReadyToShoot);
            if (player.Attack.ReadyToShoot == false)
            {
                aimTimer -= delta;
                if (aimTimer <= 0)
                {
                    (Animation as PlayerAnimation).ReadyToShoot = true;
                    player.Attack.ReadyToShoot = true;
                    aimTimer = bowDrawTimer;
                }

            }

            if ((Animation as PlayerAnimation).CurrentAttack == "Release")
            {
                (Animation as PlayerAnimation).CurrentAttack = "Draw";
                (Animation as PlayerAnimation).ReadyToShoot = false;
            }
        }

        public override void Update(double delta)
        {
            
        }
        public override void Exit()
        {
            player.Camera._AimOn = false;
			player.Attack.ReadyToShoot = false;
            (Animation as PlayerAnimation).ReadyToShoot = false;
            Movement.SetSpeed(Movement.Speed);

            (Animation as PlayerAnimation).CurrentMovementState = "Run";

            if (Actor.CurrentWeapon is Bow bow)
            {
                bow.Release();
                Animation.AbortOneShot("Bow");
            }
        }

    }
}