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
			if (Input.IsActionJustPressed(Actions.AttackLight)) WantsLightAttack(true);
			if (Input.IsActionJustReleased(Actions.AttackLight)) WantsLightAttack(false);
			if (Input.IsActionPressed(Actions.Block)) WantsBlock(true);
			if (Input.IsActionJustReleased(Actions.Block)) WantsBlock(false);
		}

		public void ReleaseArrow(string attunement)
		{
			if ((CurrentWeapon is Bow) == false) return;
			Proj = GameData.Instance.FetchProjectile(attunement+"Arrow"); 

			Proj.SpawnProjectile(Actor, CurrentWeapon.GlobalPosition, (Actor as Player).GetAimPoint());
		}
	}
}
