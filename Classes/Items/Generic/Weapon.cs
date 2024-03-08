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
		[Export] public float DefaultDamage;
		[Export] public float LightAttackStamConsumption;
		[Export] public float HeavyAttackStamConsumption;
		[Export] public float RiposteMultiplier;

		

		[ExportCategory("Gameplay")]
		[Export] public bool _CanBlock = false; 
		[Export] public bool _CanAim = false;

		[ExportCategory("Effects")]
		[Export] private Node3D Fire;
		[Export] private Node3D Lightning;
		
		public bool DamageOn = false; //Set this to true when swinging etc

		public float currentDamage;
		private List<Actor> bodiesToDamage = new List<Actor>();

        public override void _Ready()
        {
			currentDamage = DefaultDamage;
			
			if (HasNode("Area3D"))
			{
				Area3D hitArea = (Area3D) GetNode("Area3D");
				hitArea.BodyEntered += OnBodyEnteredHitArea;
				hitArea.BodyExited += OnBodyExitedHitArea;
				
				if (Wielder is AI)
					hitArea.SetCollisionMaskValue(2, false);
			}
			// if (hitArea != null)
			// {
			// 	hitArea.BodyEntered += OnBodyEnteredHitArea;
			// 	hitArea.BodyExited += OnBodyExitedHitArea;
			// 	if (Wielder is AI)
			// 		hitArea.SetCollisionMaskValue(2, false);
			// }
        }

		public void AddRiposte()
		{
			currentDamage *= RiposteMultiplier;
		}

		public void RestoreDefaultDamage()
		{
			currentDamage = DefaultDamage;
		}

		public void SetAttunement(string attun)
		{
			if (attun == Attunements.Fire)
			{
				Lightning?.Hide();
				Fire?.Show();
			}
			if(attun == Attunements.Lightning)
			{
				Fire?.Hide();
				Lightning?.Show();
			}
		}

		public void NoAttunements()
		{
			Fire?.Hide();
			Lightning?.Hide();
		}

        private void OnBodyEnteredHitArea(Node3D body)
		{
			if (body == Wielder) return;

			if (body is Actor actor)
			{
				bodiesToDamage.Add(actor);
				if (DamageOn)
				{
					foreach (Actor bodyToDamage in bodiesToDamage)
					{
						if (bodyToDamage.HasMethod("OnHit"))
						{
							bodyToDamage.OnHit(currentDamage, Wielder, Name);
						}
					}
					bodiesToDamage.Clear();

					DamageOn = false;
				}
			}
		}

		private void OnBodyExitedHitArea(Node3D body)
		{
			if (body == Wielder) return;

			if (body is Actor actor)
				if (bodiesToDamage.Contains(actor)) bodiesToDamage.Remove(actor);
		}
	}
}
