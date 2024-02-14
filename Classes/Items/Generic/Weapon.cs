using Godot;
using Globals;
using System;
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
		
		public bool _DamageOn = false; //Set this to true when swinging etc
	}
}
