using Godot;
using System;
using System.Reflection.Metadata;
using Globals;

namespace Game
{
	[GlobalClass]
	public partial class Actor : CharacterBody3D
	{	
		[Signal]
		public delegate void ActorDeathWithArgumentEventHandler(Actor actor);

		[Export] public StateMachine SMachine;
		[Export] public Stagger staggerComp;
		[Export] public Movement Movement;
		[Export] public Attack Attack;
		[Export] public Animate Animation;
		[Export] public Stamina Stam;
		[Export] public Health HP;
		[Export] protected AudioHandler audio;
		[Export] public Sprite3D LockOn;

		public bool _InAir = false;
		public bool _IsAttacking = false;

		[Export] //for animations
 		public bool _CanRotate = true;
		
		[Export] public Weapon CurrentWeapon = null;
		public Weapon CurrentOffhand = null;

		private float damageToTake;

		private float defense = 0;

		public void OnHit(float incDamage, Vector3 hitterGlobalPos, string hittingObject)
		{
			if (this is IDodger dodger)
				if (dodger.IsDodging()) return;

			damageToTake = incDamage - defense;
			if (damageToTake < 0) damageToTake = 0;
			
			if (this is IBlocker blocker)
			{
				GD.Print(ToLocal(hitterGlobalPos).Z);
				if (blocker.IsBlocking() && ToLocal(hitterGlobalPos).Z > 0)
			 	{
					blocker.BlockedAttack(damageToTake);
					return;
				}
			}
				
			HP?.TakeDamage(damageToTake);
			staggerComp?.TakeDamage(damageToTake);
			if (audio != null)
			{
				if (hittingObject == "Arrow")
					audio.Play(SoundEffects.ArrowBodyImpact);
				if (hittingObject == "Sword")
					audio.Play(SoundEffects.SwordFleshHit);
			}
			
		}

		public virtual void OnDeath()
		{
			// SMachine.SetPhysicsProcess(false);
			// SMachine.SetProcess(false);
			SetPhysicsProcess(false);
			SetProcess(false);

			EmitSignal(SignalName.ActorDeathWithArgument, this);
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
