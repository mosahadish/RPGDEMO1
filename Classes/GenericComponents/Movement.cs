using System;
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
        private Vector3 newVelocity;

        private float lerpWeight = 0f;

        public override void _EnterTree()
        {
            base._EnterTree();
            SetSpeedValues();
        }

        public override void _Ready()
        {
            base._Ready();
            lerpWeight = 0f;
        }

        public void HandleMovement(Vector3 direction, double delta)
        {
            //direction = direction.Normalized();
            
            newVelocity = actor.Velocity;
            newVelocity.X = direction.X * (CurrentSpeed * (float)delta);
            newVelocity.Z = direction.Z * (CurrentSpeed * (float)delta);

            // lerpWeight += (float)delta;
            // if (lerpWeight >= 0.25f) lerpWeight = 0.25f;

            actor.Velocity = actor.Velocity.Slerp(newVelocity, 0.15f); // For some reason 0.2f and above causes exceptions
            
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