using System.Collections.Generic;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAimingState : State
    {
        private const double bowDrawTimer = 1;

        private Player player;

        private double aimTimer;
        private bool playWeaponAnim;
        
        public override void Enter(Dictionary<string, Vector2> msg)
        {
            player??= Actor as Player;
            (Animation as PlayerAnimation).CurrentMovementState = "Walk";
            (Animation as PlayerAnimation).Aiming = true;
            (Animation as PlayerAnimation).RangeAttack("Draw");

            player.Camera.SetAiming(true);
			Movement.SetSpeed(Movement.WalkSpeed);

            if (player.CurrentWeapon is Bow)
            {
                aimTimer = bowDrawTimer;
                playWeaponAnim = true;
            }
        }

        public override void PhysicsUpdate(double delta)
        {
            HandleBow(delta);
        }

        public override void Update(double delta)
        {
            
        }
        public override void Exit()
        {
            player.Camera._AimOn = false;
			player.Attack.ReadyToShoot = false;
            (Animation as PlayerAnimation).ReadyToShoot = false;
            (Animation as PlayerAnimation).Aiming = false;
            Movement.SetSpeed(Movement.Speed);

            (Animation as PlayerAnimation).CurrentMovementState = "Run";

            if (Actor.CurrentWeapon is Bow bow)
            {
                bow.Default();
            }
            
            Animation.AbortOneShot("RangeAttack");
            player.Camera.SetAiming(false);
        }

        private void HandleBow(double delta)
        {
            if (player.CurrentWeapon is Bow bow)
            {
                Stam.DecreaseStamina(Stam.DegenRate);
                //While drawing
                if (player.Attack.ReadyToShoot == false)
                {
                    aimTimer -= delta;
                    //Finished drawing, now player is ready to shoot an arrow
                    if (aimTimer <= 0)
                    {
                        (Animation as PlayerAnimation).ReadyToShoot = true;
                        player.Attack.ReadyToShoot = true;

                        //Restart timer, and play weaponAnim = true for next release
                        aimTimer = bowDrawTimer;
                        playWeaponAnim = true;
                    }

                }

                //Play bowstring animation + audio
                if (playWeaponAnim == true && aimTimer <= 0.15)
                {
                    playWeaponAnim = false;
                    bow.Draw();
                }

                //If player just released an arrow, go back to Draw
                if ((Animation as PlayerAnimation).CurrentAttack == "Release")
                {
                    (Animation as PlayerAnimation).CurrentAttack = "Draw";
                    (Animation as PlayerAnimation).ReadyToShoot = false;
                }
            }
        }

    }
}