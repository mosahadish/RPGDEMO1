using System.Collections.Generic;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Movement : Node
    {
        // Get the gravity from the project settings to be synced with RigidBody nodes.
		public float Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
        
        public float SprintSpeed;
        public float DodgeSpeed;
        public float WalkSpeed;
        public float CurrentSpeed;
        public bool _Sprinting = false;

        [ExportCategory("MovementStats")]
        [Export] public float Speed;
        [Export] private float SprintSpeedMultiplier;
        [Export] private float WalkSpeedMultiplier;
        [Export] private float DodgeSpeedMultiplier;

        [Export] public float JumpVelocity = 4.5f;

        [ExportCategory("Dependencies")]
        [Export] private Actor actor;

        public override void _EnterTree()
        {
            base._EnterTree();
            SetSpeedValues();
        }


       

        public void HandleMovement(Vector3 direction, double delta)
        {
            Vector3 newVelocity = actor.Velocity;
            newVelocity.X = direction.X * (CurrentSpeed * (float)delta);
            newVelocity.Z = direction.Z * (CurrentSpeed * (float)delta);
    
            actor.Velocity = newVelocity;
            actor.MoveAndSlide();
        }

        private void SetSpeedValues()
        {
            SprintSpeed = Speed * SprintSpeedMultiplier;
            WalkSpeed = Speed * WalkSpeedMultiplier;
            DodgeSpeed = Speed * DodgeSpeedMultiplier;
            SetSpeed(Speed);
        }

        public virtual void SetSpeed(float speed)
        {
            CurrentSpeed = speed;
        }
    }
}