using Godot;
using Globals;
using System;
using System.Collections.Generic;
namespace Game
{
	[GlobalClass]
	public partial class Weapon : Item
	{
		[Export(PropertyHint.Enum, WeaponTypes.Offhand+
		 "," +WeaponTypes.Melee1H+ ","+WeaponTypes.Melee2H+","+ WeaponTypes.Ranged1H+","+ WeaponTypes.Ranged2H+","+ Slots.RightHand+","+ Slots.LeftHand)]
		public string SubType {get; set;}

		[ExportCategory("Stats")]
		[Export] public float Damage;
		[Export] public float LightAttackStamConsumption;
		[Export] public float HeavyAttackStamConsumption;

		[Export] public bool _CanBlock = false; 
		[Export] public bool _CanAim = false;

		[Export] private Area3D hitArea;
		
		public bool _DamageOn = false; //Set this to true when swinging etc


		private List<Actor> bodiesToDamage = new List<Actor>();

        public override void _Ready()
        {
			if (hitArea != null)
			{
				hitArea.BodyEntered += OnBodyEnteredHitArea;
				hitArea.BodyExited += OnBodyExitedHitArea;
			}

			if (Wielder is AI) hitArea.SetCollisionMaskValue(2, false);
			if (Wielder is Player) hitArea.SetCollisionMaskValue(1, false);
        }

        private void OnBodyEnteredHitArea(Node3D body)
		{
			if (body is Actor actor)
			{
				bodiesToDamage.Add(actor);
				if (_DamageOn)
				{
					foreach (Actor bodyToDamage in bodiesToDamage)
					{
						if (bodyToDamage.HasMethod("OnHit"))
							bodyToDamage.OnHit(Damage, Wielder.GlobalPosition, Name);
					}
					_DamageOn = false;
				}
			}
		}

		private void OnBodyExitedHitArea(Node3D body)
		{
			if (body is Actor actor)
				if (bodiesToDamage.Contains(actor)) bodiesToDamage.Remove(actor);
		}
	}
}
