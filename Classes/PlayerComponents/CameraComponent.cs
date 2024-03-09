using Globals;
using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
	[GlobalClass]
	public partial class CameraComponent : Node3D
	{
		[Signal] public delegate void AimWithArgumentEventHandler(bool aiming);

		[ExportCategory("Camera Settings")]
		[Export] private float deadZone;// = 0.15f;
		[Export] private float lockOnMaxAngle;// = 60.0f;
		[Export] private float lockOnMinAngle;  // = -35.0f;
		[Export] private float hSensitivity;
		[Export] private float vSensitivity;
		[Export] private Vector3 defaultPosition;
		[Export] private Vector3 aimPosition;

		[ExportCategory("Dependencies")]
		[Export] Player player;
		[Export] private Camera3D camera;
		[Export] private SpringArm3D cameraContainer;
		[Export] PlayerStateMachine sMachine;
		[Export] public LockOnComponent lockOnComponent;
		
		public Actor Target;

		public bool _LockOn = false;
		public bool _AimOn = false;

		private Vector2 rotationDir;
		private Vector3 newRotation;
		private Vector3 newPlayerRotation;
		private Vector3 newPos;
		private Basis newBasis;
		
		private float distToTarget = 500;
		private float tempDistance = 500;

		private float targetAngle = 0;
		private float desiredRotationX = 0;
		private float desiredRotationY = 0;

		private List<Actor> possibleTargets = new();
		private float weight = 0;

		private Vector3 originalRotation;

		private Tween tweener;

        public override void _Ready()
        {
            if (lockOnComponent != null)
			{	
				lockOnComponent.camera = this;
				lockOnComponent.TargetChanged += TargetChanged;
			}

			originalRotation = Rotation;
        }

		public void ResetRotation()
		{
			Rotation = originalRotation;
		}

        public override void _Process(double delta)
		{
			//Since camera is Top Level, we need to manually follow
			FollowPlayer();
			rotationDir = Input.GetVector("rotate_right", "rotate_left", "rotate_up", "rotate_down", deadZone);
			
			if (_AimOn) AimedRotation(delta);
			else if (_LockOn) LockedRotation(delta);
			else FreeRotation(delta);

			if (Input.IsActionJustPressed(Actions.Aim)) sMachine.HandleCameraInput(Actions.Aim);
			if (Input.IsActionJustReleased(Actions.Aim)) sMachine.HandleCameraInput(Actions.AimCancel);
			if (Input.IsActionJustPressed(Actions.LockOn) && _LockOn == true) sMachine.HandleCameraInput(Actions.LockOff);
			else if (Input.IsActionJustPressed(Actions.LockOn)) sMachine.HandleCameraInput(Actions.LockOn);

			if (_LockOn)
			{
				if (Input.IsActionJustPressed(Actions.NextRightTarget)) lockOnComponent.FetchRightTarget();
				if (Input.IsActionJustPressed(Actions.NextLeftTarget)) lockOnComponent.FetchLeftTarget();
			}
		}


		/*
		Called from the StateMachine, in response to Aim input
		Zooms in/out according to the input
		*/
		public void SetAiming(bool aiming)
		{
			_AimOn = aiming;
			EmitSignal(SignalName.AimWithArgument, aiming);
			if (tweener != null)
			{
				tweener.Kill();
			}
			tweener = CreateTween();

			if (aiming)
			{
				//cameraContainer.Position = aimPosition;
				tweener.TweenProperty(cameraContainer, "position", aimPosition, 0.5);
			}
			else if (aiming == false)
			{
				//cameraContainer.Position = defaultPosition;
				tweener.TweenProperty(cameraContainer, "position", defaultPosition, 0.3);
			}
		}


		/*
		Called by the StateMachine when locking on
		*/
        public bool FindClosestTarget()
		{
			Target = lockOnComponent.FetchClosestTarget();
			
			return Target != null;
		}


		/*
		Handles LockedRotation, look at target as long as it's not null and alive
		*/
		private void LockedRotation(double delta)
		{
			if (Target != null && Target.Dead == false)
				{
					LookAtTarget(delta);
				}
			else 
			{
				ReleaseLockOn();
				_LockOn = false;
			}
		}

		public void LockOnTarget()
		{
			_LockOn = true;
			Target.LockOn.Show();
			lockOnComponent.Targetted(Target);
		}

		private void TargetChanged(Actor newTarget)
		{
			weight = 0;
			if (newTarget == null) 
			{
				Target?.LockOn.Hide();
				Target = null;
			}
			if (Target != newTarget)
			{
				Target.LockOn.Hide();
				Target = newTarget;
				Target.LockOn.Show();
			}
		}

		public void ReleaseLockOn()
		{
			_LockOn = false;
			lockOnComponent.Targetted(null);
		}

		private void LookAtTarget(double delta)
		{
			if (Target == null) return;

			newRotation = GlobalPosition.DirectionTo(Target.GlobalPosition); //new rotatoin Dir
			targetAngle = Mathf.Atan2(newRotation.X, newRotation.Z);
			
			if (player.Movement._Sprinting == false && player.IsAttacking() == false)
				player.LookInDirection(newRotation);

			newBasis = Target.GlobalTransform.LookingAt(GlobalTransform.Origin, Vector3.Up).Basis;

			weight += (float)delta;
			
			if (weight >= 1) weight = 1f;
			GlobalBasis = GlobalBasis.Slerp(newBasis, weight);
		}

		private void AimedRotation(double delta)
		{
			HandleRotations(delta);
			newPlayerRotation = player.Rotation;
			newPlayerRotation.Y = (float)Mathf.LerpAngle(newPlayerRotation.Y, Rotation.Y, 0.99);

			player.Rotation = newPlayerRotation;
		}

		public void FreeRotation(double delta)
		{
			HandleRotations(delta);
		}

		private void HandleRotations(double delta)
		{
			newRotation = Rotation;
			newRotation.Y = Rotation.Y + (hSensitivity * rotationDir.X * (float)delta);
			newRotation.X = Rotation.X + (vSensitivity * rotationDir.Y * (float)delta);
			newRotation.X = Mathf.Clamp(newRotation.X, Mathf.DegToRad(-50), Mathf.DegToRad(70));

			Rotation = newRotation;
		}

		private void FollowPlayer()
		{
			newPos = Position;
			newPos.X = (float)Mathf.Lerp(newPos.X, player.Position.X, 0.99);
			newPos.Y = (float)Mathf.Lerp(newPos.Y, player.Position.Y, 0.99);
			newPos.Z = (float)Mathf.Lerp(newPos.Z, player.Position.Z, 0.99);

			Position = newPos;
		}

	}

}
