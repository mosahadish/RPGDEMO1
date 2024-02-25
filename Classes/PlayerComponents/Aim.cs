using Godot;
using System;

namespace Game
{
	[GlobalClass]
	public partial class Aim : Node3D
	{
		[Export] private Camera3D camera;
		[Export] private RayCast3D rayAim;
		[Export] private Marker3D markerAim;

		private Vector3 aimTarget;
		[Export] private TextureRect aim;

		Color red = new Color(Colors.Red);
		Color white = new Color(Colors.White);

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (rayAim.IsColliding())
			{
				aimTarget = rayAim.GetCollisionPoint();
				if (rayAim.GetCollider() is AI)
					aim.Modulate = red;
				else aim.Modulate = white;
			}
			else aimTarget = markerAim.GlobalPosition;
		}

		public Vector3 GetAimPoint()
		{
			return aimTarget;
		}
	}

}
