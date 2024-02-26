using System.Collections.Generic;
using Globals;
using Godot;
namespace Game
{
	[GlobalClass]
	public partial class PlayerAttack : Attack
	{
		public override void _Ready()
		{
			base._Ready();
		}
		public override void _PhysicsProcess(double _delta)
		{
			if (Input.IsActionJustPressed(Actions.LightAttack)) WantsLightAttack(true);
			if (Input.IsActionJustReleased(Actions.LightAttack)) WantsLightAttack(false);
			if (Input.IsActionPressed(Actions.Block)) WantsBlock(true);
			if (Input.IsActionJustReleased(Actions.Block)) WantsBlock(false);
			if (Input.IsActionJustPressed(Actions.Parry)) WantsParry(true);
		}

		public void ReleaseArrow(string attunement)
		{
			if ((CurrentWeapon is Bow) == false) return;
			Proj = GameData.Instance.FetchProjectile(attunement+"Arrow"); 

			//Proj.Basis = CurrentWeapon.GlobalTransform.Basis;
			Proj.SpawnProjectile(Actor, CurrentWeapon.GlobalPosition, (Actor as Player).GetAimPoint());
		}
	}
}
