using Godot;
using System;
using System.Reflection.Metadata;
using Globals;
using System.Threading.Tasks;

namespace Game
{
	[GlobalClass]
	public partial class Actor : CharacterBody3D
	{	
		[Signal]
		public delegate void ActorDeathWithArgumentEventHandler(Actor actor);

		[Signal]
		public delegate void ActorGotParriedEventHandler();
		[Signal]
		public delegate void ActorGotStaggeredEventHandler();

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

		[Export] public bool _CanRotate;
 		
		
		[Export] public Weapon CurrentWeapon = null;
		public Weapon CurrentOffhand = null;

		private float damageToTake;

		private float defense = 0;

		private Timer deathTimer = new();

		public bool Dead;

        public override void _Ready()
        {
		
        }

        public void OnHit(float incDamage, Actor hitter, string hittingObject)
		{
			if (Dead) return;

			if (this is IDodger dodger)
				if (dodger.IsDodging()) return;

			damageToTake = incDamage - defense;
			if (damageToTake < 0) damageToTake = 0;
			
			if (HasOffhand() && CurrentOffhand is ParryingObject parry)
			{
				if (parry.IsActive())
				{
					parry.DeactivateParryWindow();
					hitter.GotParried();
					return;
				}
			}

			if (this is IBlocker blocker)
			{
				if (blocker.IsBlocking() && ToLocal(hitter.GlobalPosition).Z > 0)
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
			Dead = true;
			SetPhysicsProcess(false);
			SetProcess(false);
			

			if (this is AI ai)
			{
				ai.SMachine.SetPhysicsProcess(false);
				ai.SMachine.SetProcess(false);
				ai.SMachine.TransitionTo(nameof(AIDeathState));
			}
			else
			{
				SMachine.SetPhysicsProcess(false);
				SMachine.SetProcess(false);
				SMachine.TransitionTo(nameof(PlayerDeathState), null);
			}
	
			audio?.Play("Death");
			EmitSignal(SignalName.ActorDeathWithArgument, this);
		}


		public bool HasWeapon()
		{
			return CurrentWeapon != null;
		}

		public void WeaponDamageOn()
		{
			if (HasWeapon()) CurrentWeapon.DamageOn = true;
		}

		public void WeaponDamageOff()
		{
			if (HasWeapon()) CurrentWeapon.DamageOn = false;
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

		public bool HasParryingWeapon()
		{
			if (HasOffhand() == false) return false;
			return CurrentOffhand is ParryingObject;
		}

		public void LookInDirection(Vector3 Dir, bool ignoreRotate = false)
        {
			if (ignoreRotate == false)
				if (_CanRotate == false) return;
			
            float TargetAngle = Mathf.Atan2(Dir.X, Dir.Z);
            Vector3 newRotation = Rotation;
            newRotation.Y = (float)Mathf.LerpAngle(Rotation.Y, TargetAngle, 0.1);

            Rotation = newRotation;
        }

		public virtual void GotParried()
		{
			audio.Play("ParrySuccess");
			EmitSignal(SignalName.ActorGotParried);
		}

		public virtual void GotStaggered()
		{
			audio.Play("Staggered");
			EmitSignal(SignalName.ActorGotStaggered);
		}
    }
}
