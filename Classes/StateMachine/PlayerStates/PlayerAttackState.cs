using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAttackState : State
    {
        private Vector2 inputDir;
        private Vector3 Direction;
        private int chain = 1;

        private CameraComponent camera;
    
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Enter(Dictionary<string, Vector2> Msg)
        {
            Actor._IsAttacking = true;
            if (Msg.ContainsKey(Actions.AttackLight))
            {
                
                chain = (int)Msg[Actions.AttackLight].Y;

                if (chain == 1) Anim = WeaponName+Animations.Attack1;
                if (chain == 2) Anim = WeaponName+Animations.Attack2;
                if (chain ==3) Anim = WeaponName+Animations.Attack3;
                
                Stam.DecreaseStamina(Actor.CurrentWeapon.LightAttackStamConsumption);
                Stam.Regen = false;

                Animation.Transition(AnimTransition, Anim);
                Animation.OneShot(WeaponName);
            }
            // global_transform.basis.z instaed of PositiveZ marker? NOT WORKING
            if (InputDir == Vector2.Zero) Direction = ((Actor as Player).PositiveZ.GlobalPosition - Actor.GlobalPosition).Normalized();
            else 
            {
                Direction = new Vector3(InputDir.X, 0, InputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y).Normalized();
                GD.Print(InputDir);
                InputDir = Vector2.Zero;
            }
        }


        public override void PhysicsUpdate(double delta)
        {
            // GD.Print(InputDir);
            LookInDirection(Direction);
            Movement.HandleMovement(Direction, delta);
        }

        public override void Update(double delta)
        {
           
        }

        private void LookInDirection(Vector3 Dir)
        {
            float TargetAngle = Mathf.Atan2(Dir.X, Dir.Z);
            Vector3 newRotation = Actor.Rotation;
            newRotation.Y = (float)Mathf.LerpAngle(Actor.Rotation.Y, TargetAngle, 0.2);

            Actor.Rotation = newRotation;
        }

        public void SetCamera(CameraComponent camera)
		{
			this.camera = camera;
		}

        public override void Exit()
        {
            Stam.Regen = true;
            Movement.SetSpeed(Movement.Speed);
            Actor._IsAttacking = false;
        }
    }
}