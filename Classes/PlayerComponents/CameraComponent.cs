using Globals;
using Godot;
using System;

namespace Game
{
	[GlobalClass]
	public partial class CameraComponent : Node3D
	{

		[Export] private float hSensitivity;
		[Export] private float vSensitivity;
		[Export] Player player;
		[Export] PlayerStateMachine sMachine;

		public bool _LockOn = false;
		public bool _AimOn = false;

		private Vector2 rotationDir;
		private Vector3 newRotation;
		private Vector3 newPlayerRotation;

		private Vector3 newPos;
		public override void _Process(double delta)
		{
			//Since camera is Top Level, we need to manually follow
			FollowPlayer();
			rotationDir = Input.GetVector("rotate_right", "rotate_left", "rotate_up", "rotate_down");
			
			if (Input.IsActionJustPressed(Actions.Aim)) sMachine.HandleCameraInput(Actions.Aim);
			if (Input.IsActionJustReleased(Actions.Aim)) sMachine.HandleCameraInput(Actions.AimCancel);

			if (_AimOn) AimedRotation(delta);
			else if (_LockOn) LockedRotation(delta);
			else FreeRotation(delta);
		}


		private void LockedRotation(double delta)
		{

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
			newPos.X = Mathf.Lerp(newPos.X, player.Position.X, 1);
			newPos.Y = Mathf.Lerp(newPos.Y, player.Position.Y, 1);
			newPos.Z = Mathf.Lerp(newPos.Z, player.Position.Z, 1);

			Position = newPos;
		}
	}

}
