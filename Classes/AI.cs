using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor
	{
        [Export] public Actor Target;
		[Export] public Raycasts Raycasts;


		private Vector3 desiredVelo;
		

		public void ApplySteeringForce(Vector3 globalTargetPos, double delta)
		{
			desiredVelo = (globalTargetPos - GlobalPosition);
			Velocity = desiredVelo + Velocity;
		}
    }
}