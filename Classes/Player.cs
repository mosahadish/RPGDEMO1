using Godot;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public delegate void NotifyValueChange(float value);
	public delegate void NotifyAttunement(string msg);
	
	
	[GlobalClass]
	public partial class Player : Actor, IBlocker, IMeleeAttacker, IDodger
	{
		/*
		The player class is heavily dependent upon the classes in the category Dependencies
		Each of the classes represent a mechanic;

		CameraComponent - Rotation, Aiming, Lock On
		PlayerMovement - Moving, Dodging, Jumping (inherits from Movement, has additional methods)
		Interact - Picking up items (In the future interact with objects?)
		Inventory - Inventory UI, move items around, use items etc
		EquipmentHandler - Works alongside Inventory, this represents the items currently equipped and are displayed on the player character
		Aim - Handles the aim point when Aiming so we can shoot projectiles
		*/

		#region Events

		public event NotifyAttunement AttunementChanged;

		#endregion

		#region Exported members
		[Export(PropertyHint.Enum, Attunements.Lightning+
		 "," + Attunements.Fire)]
		public string CurrentAttunement {get; set;} = Attunements.Fire;

		[ExportCategory("Dependencies")]
		[Export] public CameraComponent Camera;
		[Export] public new PlayerMovement Movement;
		[Export] public AreaInteract Interact;
		[Export] private PlayerEquipmentHandler equip;
		[Export] Inventory Inventory;
		[Export] private Aim aim;

		#endregion

		#region Private members

		private int AttunementIterator = 0;
		private string[] arrAttunement = {Attunements.Fire, Attunements.Lightning};
		private Vector3 frontDirection;

		private bool _isBlocking = false;
		bool IBlocker.Blocking
    	{
        get { return _isBlocking; }
        set { _isBlocking = value; }
    	}

        private bool _attackBlocked = false;
		bool IBlocker.AttackBlocked
    	{
        get { return _attackBlocked; }
        set { _attackBlocked = value; }
    	}

		private bool _isDodging = false;
		bool IDodger.Dodging
    	{
        get { return _isDodging; }
        set { _isDodging = value; }
    	}

		#endregion

        public override void _Ready()
		{
			base._Ready();

			HandleChildrenDependencies();

			_isBlocking = false;
			AttunementChanged?.Invoke(CurrentAttunement);

			// Set front direction, makes it easier to face ahead when required
			frontDirection.X = Position.X;
			frontDirection.Y = Position.Y;
			frontDirection.Z = Position.Z + 1;
		}

		private void HandleChildrenDependencies()
		{
			if (SMachine is PlayerStateMachine machine)
			{
				if (Inventory != null && Interact != null)
				{
					Interact.PickedUpItemWithArgument += Inventory.AddItem;
				}
				if (equip != null)
				{
					equip.WeaponChanged += machine.OnWeaponChanged;
					equip.OffhandChanged += machine.OnOffhandChanged;
					equip.Inventory = Inventory;
					equip.Player = this;
				}
				AttunementChanged += machine.OnAttunementChanged;

				if (Stam != null)
					Stam.StaminaChanged += machine.OnStaminaChanged;

				if (audio !=null)
					equip.Audio = audio;
			}
		}

		private void DisconnectEvents()
		{
			if (SMachine is PlayerStateMachine machine)
			{
				if (Inventory != null)
				{
					Interact.PickedUpItemWithArgument -= Inventory.AddItem;
				}
				if (equip != null)
				{
					equip.WeaponChanged -= machine.OnWeaponChanged;
					equip.OffhandChanged -= machine.OnOffhandChanged;
				}
				AttunementChanged -= machine.OnAttunementChanged;

				if (Stam != null)
					Stam.StaminaChanged -= machine.OnStaminaChanged;
			}
		}

        public override void _ExitTree()
        {
            base._ExitTree();
			DisconnectEvents();
        }

        public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed(Actions.ChangeAttunement) && _IsAttacking == false && Attack.ReadyToShoot == false)
			{
				AttunementIterator +=1;
				if (AttunementIterator == arrAttunement.Length) AttunementIterator = 0;
				CurrentAttunement = arrAttunement[AttunementIterator];

				if (HasWeapon())
				{
					CurrentWeapon.SetAttunement(CurrentAttunement);
				}

				AttunementChanged?.Invoke(CurrentAttunement);
			}
			
		}

		public Vector3 GetGlobalFrontDir()
		{
			return ToGlobal(frontDirection);
		}

		public Vector3 GetAimPoint()
		{
			return aim.GetAimPoint();
		}

		public void Sprint()
		{
			(Animation as PlayerAnimation).MovementTransition("Sprint");
			Movement._Sprinting = true;
			Movement.SetSpeed(Movement.SprintSpeed);
			Stam.Regen = false;
			Stam.Degen = true;
		}

		public void ReleaseSprint()
		{
			(Animation as PlayerAnimation).MovementTransition("Run");
			Movement._Sprinting = false;
			Movement.SetSpeed(Movement.Speed);
			Stam.Regen = true;
			Stam.Degen = false;
		}

		#region Block

		public void Block()
        {
            Animation.Transition("Shield", Animations.Block);
			if (HasWeapon())
				Animation.Transition(CurrentWeapon.Name+Animations.TransitionMovement, CurrentWeapon.Name + Animations.Walk);
			else Animation.Transition(Animations.TransitionMovement, Animations.Walk);
			_isBlocking = true;
			Stam.Degen = true;
			Stam.Regen = false;
        }

		public void BlockedAttack(float damageToTake)
        {
            Animation.Transition("Shield", Animations.BlockedAttack);
			audio.Play(SoundEffects.ShieldBlock);
			_attackBlocked = true;
			Stam.DecreaseStamina(damageToTake);
        }

		public void BlockCounterAttack()
		{
			BlockRelease();
			_CanRotate = false;
			_IsAttacking = true;
			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.CounterAttack);
			Animation.OneShot(CurrentWeapon.Name);
			//Audio.PlayAudio(SoundEffects.ShieldBlock);
			
			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
			_attackBlocked = false;
			//_IsBlocking = false;
		}

        public void BlockHold()
        {
			_isBlocking = true;
			_attackBlocked = false;
            Animation.Transition("Shield", Animations.BlockHold);
        }

        public void BlockRelease()
        {
			if (_isBlocking == false) return;
			
			_attackBlocked = false;
			Animation.Transition("Shield", Animations.BlockRelease);
			if (HasWeapon())
				Animation.Transition(CurrentWeapon.Name+Animations.TransitionMovement, CurrentWeapon.Name+Animations.Movement);
			else Animation.Transition(Animations.TransitionMovement, Animations.Movement);
            
			_isBlocking = false;
			Stam.Degen = false;
			Stam.Regen = true;
        }

        public bool IsBlocking()
        {
            return _isBlocking;
        }
		public bool CanCounter()
        {
            return _attackBlocked;
        }

		#endregion
		
		#region Attack

        public void Attack1()
        {
            _IsAttacking = true;
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack1);
			// Animation.OneShot(CurrentWeapon.Name);
			(Animation as PlayerAnimation).MainAttackTransition("Attack1");
			(Animation as PlayerAnimation).RequestOneShot("MainAttack");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack1);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
        }

        public void Attack2()
        {
            _IsAttacking = true;
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack2);
			// Animation.OneShot(CurrentWeapon.Name);
			(Animation as PlayerAnimation).MainAttackTransition("Attack2");
			(Animation as PlayerAnimation).RequestOneShot("MainAttack");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack2);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
        }

        public void Attack3()
        {
            _IsAttacking = true;
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack3);
			// Animation.OneShot(CurrentWeapon.Name);
			(Animation as PlayerAnimation).MainAttackTransition("Attack3");
			(Animation as PlayerAnimation).RequestOneShot("MainAttack");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack3);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
		}

		public void ComboAttack()
		{
			
		}

		public void SprintLightAttack()
        {
			_CanRotate = false;
            _IsAttacking = true;
			Movement._Sprinting = false;
			Stam.Degen = false;

			(Animation as PlayerAnimation).MainAttackTransition(Animations.SprintLightAttack);
			(Animation as PlayerAnimation).RequestOneShot("MainAttack");
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.SprintLightAttack);
			// //Animation.Transition(CurrentWeapon.Name + Animations.Movement, CurrentWeapon.Name+Animations.SprintLightAttack);
			// Animation.OneShot(CurrentWeapon.Name);
        }

		public void SprintHeavyAttack()
        {
            // _IsAttacking = true;
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.SprintAttack);
			// Animation.OneShot(CurrentWeapon.Name);
        }

        public void DodgeLightAttack()
        {
            _CanRotate = false;
            _IsAttacking = true;
			Movement._Sprinting = false;
			Stam.Regen = false;
			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);

			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.DodgeLightAttack);
			Animation.OneShot(CurrentWeapon.Name);
        }

        public void JumpAttack()
        {

        }

        public void HeavyAttack()
        {

        }

		public void FinishAttacking()
		{
			_IsAttacking = false;
			_CanRotate = true;
			Stam.Regen = true;
		}

        public bool IsAttacking()
        {
            return _IsAttacking;
        }
		#endregion

		#region Dodge

        public void Dodge()
        {
			Stam.DecreaseStamina(Stam.DodgeConsumption);
            Stam.Regen = false;
            Movement.CurrentSpeed = Movement.DodgeSpeed;
            _isDodging = true;
            _CanRotate = false;
			if (HasWeapon())
            	Animation.Transition(CurrentWeapon.Name + Animations.TransitionMovement, CurrentWeapon.Name + Animations.Dodge);
			else Animation.Transition(Animations.TransitionMovement, Animations.Dodge);
        }

		public void FinishDodging()
		{
			Movement.CurrentSpeed = Movement.Speed;
            Stam.Regen = true;
            _isDodging = false;
            _CanRotate = true;
		}

        public bool IsDodging()
        {
            return _isDodging;
        }

		#endregion
    
	}
}

