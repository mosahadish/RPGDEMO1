using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class Actor : CharacterBody3D
	{
		
		[Export] public StateMachine SMachine;
		[Export] public Movement Movement;
		[Export] public Attack Attack;
		[Export] public Animate Animation;
		[Export] public Stamina Stam;


		public bool _IsDodging = false;
		public bool _InAir = false;
		public bool _IsBlocking = false;
		public bool _IsAttacking = false;
		public bool _BlockedAttack = false; //For counter attacking
		
		public Weapon CurrentWeapon = null;
		public Weapon CurrentOffhand = null;

		public void OnHit(GenericParameter hitter)
		{

		}

		public bool HasWeapon()
		{
			return CurrentWeapon != null;
		}

		public bool HasOffhand()
		{
			return CurrentOffhand != null;
		}

		public bool HasBlockWeapon()
		{
			if (HasOffhand() == false) return false;
			return CurrentOffhand._CanBlock == true;
		}

		public bool HasAimingWeapon()
		{
			if (HasWeapon() == false) return false;
			return CurrentWeapon._CanAim;
		}
	}
}
