using Godot;
using System;
using System.Collections.Generic;
using Globals;

namespace Game
{
	[GlobalClass]
	public partial class PlayerRunState : State
	{
		private Vector3 moveDir;
		private Vector3 newVelo;
		private Vector3 newDir;
		private CameraComponent camera;

		private Vector2 newInputDir; //Only for Blend position, need to do X*-1

        public override void _Ready()
        {
            base._Ready();
			AnimTransition = Animations.TransitionMovement;
			Anim = Animations.Movement;
        }

        public override void Enter(Dictionary<string, Vector2> Msg)
		{
		
			if (HandleSprint(Msg) == false)
			{
				SetAnim(Animations.TransitionMovement, Animations.Movement);
				Movement.SetSpeed(Movement.Speed);
			}
			

			if (Msg.ContainsKey("input_dir")) InputDir = Msg["input_dir"];

			Animation.Transition(AnimTransition, Anim);
		}


		public override void PhysicsUpdate(double delta)
		{
			moveDir = CalcMoveDirection(InputDir);
			//Running around according to the camera's direction
			if ((!InputDir.Equals(Vector2.Zero) && camera._LockOn == false && camera._AimOn == false) || Movement.Sprinting)
			{
				InputDir = new Vector2(0, InputDir.Length());
				(Movement as PlayerMovement).HandleFreeMovement(moveDir, delta);
			}

			else
			{
				Movement.HandleMovement(moveDir ,delta);
			}
			//if (Anim.Contains(Animations.Movement) || Anim.Contains(Animations.Walk)) //basically if not sprinting
			if (Movement.Sprinting == false)
				{
					InputDir.X = -1*InputDir.X;
					Animation.BlendPosition(Anim, InputDir);
				}
		}


		public override void Update(double delta)
		{

		}

		public override void Exit()
		{

		}

		private Vector3 CalcMoveDirection(Vector2 InputDir)
		{
			newDir.X = InputDir.X;
			newDir.Y = 0;
			newDir.Z = InputDir.Y;

			//Rotate the vector around the camera, so we follow the camera's direction
			newDir = newDir.Rotated(Vector3.Up, (Actor as Player).Camera.Rotation.Y);
			//Multiply the direction by our input strength, gives more control over acceleration
			newDir *= InputDir.Length();
			return newDir;
		}

		public void SetCamera(CameraComponent camera)
		{
			this.camera = camera;
		}

		private bool HandleSprint(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Sprint)) 
			{
				SetAnim(Animations.TransitionMovement, Animations.Sprint);
				Movement.Sprinting = true;
				Movement.SetSpeed(Movement.SprintSpeed);
				Stam.Regen = false;
				Stam.Degen = true;
				return true;
			}

			if (Msg.ContainsKey(Actions.SprintRelease)) 
			{
				SetAnim(Animations.TransitionMovement, Animations.Movement);
				Movement.Sprinting = false;
				Movement.SetSpeed(Movement.Speed);
				Stam.Regen = true;
				Stam.Degen = false;
				return true;
			}
			return false;
		}
	}
}
