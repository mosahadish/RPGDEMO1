using Globals;
using Godot;
using System;
using System.Collections.Generic;

namespace Game
{
	[GlobalClass]
	public partial class CameraComponent : Node3D
	{
		[ExportCategory("Camera Settings")]
		[Export] private float deadZone = 0.15f;
		[Export] private float lockOnMaxAngle = 60.0f;
		[Export] private float lockOnMinAngle = -35.0f;
		[Export] private float hSensitivity;
		[Export] private float vSensitivity;

		[ExportCategory("Dependencies")]
		[Export] Player player;
		[Export] private Camera3D camera;
		[Export] PlayerStateMachine sMachine;
		
		public Actor Target;

		public bool _LockOn = false;
		public bool _AimOn = false;

		private Vector2 rotationDir;
		private Vector3 newRotation;
		private Vector3 newPlayerRotation;
		private Vector3 newPos;
		
		private float distToTarget = 500;
		private float tempDistance = 500;

		private float targetAngle = 0;
		private float desiredRotationX = 0;
		private float desiredRotationY = 0;

		private List<Actor> possibleTargets = new();

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
		}


		public bool FindClosestTarget()
		{
			if (possibleTargets.Count >=1) 
			{
				Target = possibleTargets[0];
				distToTarget = Position.DistanceSquaredTo(Target.Position);
			}

			foreach (Actor possibleTarget in possibleTargets)
			{
				tempDistance = Position.DistanceSquaredTo(possibleTarget.Position);
				if (distToTarget > tempDistance)
				{
					distToTarget = tempDistance;
					Target = possibleTarget;
				}
			}
			
			return Target != null;
		}

		private void LockedRotation(double delta)
		{
			if (Target != null)
				{
					LookAtTarget();
				}
			else _LockOn = false;
		}

		public void LockOnTarget()
		{
			_LockOn = true;
			Target.LockOn.Show();
		}

		public void ReleaseLockOn()
		{
			_LockOn = false;
			Target.LockOn.Hide();
			Target = null;
			distToTarget = 500;
			tempDistance = 500;
		}

		private void LookAtTarget()
		{
			if (Target == null) return;

			newRotation = GlobalPosition.DirectionTo(Target.GlobalPosition); //new rotatoin Dir
			targetAngle = Mathf.Atan2(newRotation.X, newRotation.Z);
			
			if (player.Movement.Sprinting == false)
				player.LookInDirection(newRotation);
			newRotation = Rotation;
			newRotation.Y = (float)Mathf.LerpAngle(newRotation.Y, targetAngle, 0.8);
		
			Rotation = newRotation;

			//Try stuff out for vertical lock on.. is it working? IDK
			distToTarget = camera.GlobalPosition.DistanceTo(Target.GlobalPosition);
			
			rotationDir.X = GetViewport().GetVisibleRect().Size.X/2;//target.GlobalPosition.X;
			rotationDir.Y = GetViewport().GetVisibleRect().Size.Y/4;
			
			newRotation = camera.ProjectPosition(rotationDir, distToTarget);

			desiredRotationX = Rotation.X + Mathf.Atan2(Target.GlobalPosition.Y - newRotation.Y, distToTarget);
			desiredRotationX = (float)Mathf.Clamp(desiredRotationX, -35.0, 60.0);

			newRotation = RotationDegrees;
			newRotation.X = (float)Mathf.Lerp(RotationDegrees.X, desiredRotationX, 0.1);

			RotationDegrees = newRotation;
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

		public void OnArea3DEntered(Node3D body)
		{
			if (body is AI)
			{
				possibleTargets.Add((Actor)body);
			}
		}

		public void OnArea3DExited(Node3D body)
		{
			if (body is AI)
			{
				if (possibleTargets.Contains((Actor)body))
				{
					possibleTargets.Remove((Actor)body);
				}

				if (body == Target)
				{
					ReleaseLockOn();
				}
			}
		}
	}

}
