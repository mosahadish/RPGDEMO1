using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class Actor : CharacterBody3D
	{	
		[Signal]
		public delegate void ActorDeathWithArgumentEventHandler(Actor actor);

		[Export] public StateMachine SMachine;
		[Export] public Movement Movement;
		[Export] public Attack Attack;
		[Export] public Animate Animation;
		[Export] public Stamina Stam;
		[Export] public Health HP;
		[Export] public Sprite3D LockOn;


		public bool _IsDodging = false;
		public bool _InAir = false;
		public bool _IsBlocking = false;
		public bool _IsAttacking = false;
		public bool _BlockedAttack = false; //For counter attacking
		public bool _CanRotate = true;
		
		public Weapon CurrentWeapon = null;
		public Weapon CurrentOffhand = null;

		private float damageToTake;

		private float defense = 0;

		public void OnHit(float incDamage, Vector3 hitterGlobalPos, string hittingObject)
		{
			if (_IsDodging) return;

			damageToTake = incDamage - defense;
			if (damageToTake < 0) damageToTake = 0;
			
			if (_IsBlocking && ToLocal(hitterGlobalPos).Z > 0)
			{
				//play block audio
				//play attack blocked animation
				(this as IBlocker).BlockHold();
				Stam.DecreaseStamina(damageToTake);
			}

			//else take hp damage, stagger damage, play hit audio 
			else
			{
				HP.TakeDamage(damageToTake);
			}
		}

		public virtual void OnDeath()
		{
			EmitSignal(SignalName.ActorDeathWithArgument, this);

			SetPhysicsProcess(false);
			SetProcess(false);
			SMachine.SetPhysicsProcess(false);
			SMachine.SetProcess(false);
		}


		public bool HasWeapon()
		{
			return CurrentWeapon != null;
		}

		public void WeaponDamageOn()
		{
			if (HasWeapon()) CurrentWeapon._DamageOn = true;
		}

		public void WeaponDamageOff()
		{
			if (HasWeapon()) CurrentWeapon._DamageOn = false;
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

		public void LookInDirection(Vector3 Dir)
        {
			if (_CanRotate == false) return;
			
            float TargetAngle = Mathf.Atan2(Dir.X, Dir.Z);
            Vector3 newRotation = Rotation;
            newRotation.Y = (float)Mathf.LerpAngle(Rotation.Y, TargetAngle, 0.2);

            Rotation = newRotation;
        }
    }
}
