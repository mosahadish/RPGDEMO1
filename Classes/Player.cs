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

		#region Events/Signals

		public event NotifyAttunement AttunementChanged;
		[Signal]
		public delegate void PlayerRestingEventHandler();
		[Signal]
		public delegate void ItemUseSuccessEventHandler();

		#endregion

		#region Exported members
		[Export(PropertyHint.Enum, Attunements.Lightning +
		 "," + Attunements.Fire)]
		public string CurrentAttunement { get; set; } = Attunements.Fire;

		[ExportCategory("Dependencies")]
		[Export] public CameraComponent Camera;
		[Export] public new PlayerMovement Movement;
		[Export] public AreaInteract Interact;
		[Export] public PlayerEquipmentHandler Equip;
		[Export] Inventory Inventory;
		[Export] private Aim aim;

		#endregion
		public bool Consuming = false;
		public bool CanInteract = true;

		#region Private members

		
		public bool Resting = false;
		public Checkpoint lastVisitedCheckpoint;
		private int AttunementIterator = 0;
		private string[] arrAttunement = { Attunements.Fire, Attunements.Lightning };
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

			if (GetParent() is Map map)
			{
				ActorDeathWithArgument += map.OnPlayerDeath;
			}
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
			if (Inventory != null && Interact != null)
			{
				Interact.PickedUpItemWithArgument += Inventory.AddItem;
				Interact.PickedUpItemWithArgument += PickedUpItem;
				Inventory.UsedItemWithArgument += OnInventoryUsedItem;
				ItemUseSuccess += Inventory.ItemUsed;
			}
			if (SMachine is PlayerStateMachine machine)
			{
				if (Equip != null)
				{
					Equip.WeaponChanged += machine.OnWeaponChanged;
					Equip.OffhandChanged += machine.OnOffhandChanged;
					Equip.Inventory = Inventory;
					Equip.Player = this;
				}
				AttunementChanged += machine.OnAttunementChanged;

				if (staggerComp != null)
					ActorGotStaggered += SMachine.OnStagger;

				if (Stam != null)
					Stam.StaminaChanged += machine.OnStaminaChanged;

				if (audio != null)
					Equip.Audio = audio;

				ItemUseSuccess += machine.OnItemUse;
			}

			if (Interact != null)
			{
				Interact.Player = this;
			}
		}

        private void PickedUpItem(Item i)
        {
			SMachine.TransitionTo(nameof(PlayerPickUpState), null);
        }


        private void OnInventoryUsedItem(Item item)
        {
			if (_IsAttacking == true || _isDodging == true || _isBlocking == true || _InAir == true || Consuming == true) return;
			
            if (item is Consumable consumable)
			{
				if (consumable.ConsumableType == ConsumableTypes.Heal)
				{
					HP.Heal(consumable.Consume());
					Consuming = true;
					//SMachine.TransitionTo(nameof(PlayerDrinkState), null);
					EmitSignal(SignalName.ItemUseSuccess);
				}
			}
        }

        private void DisconnectEvents()
		{
			if (Inventory != null)
			{
				Interact.PickedUpItemWithArgument -= Inventory.AddItem;
				Inventory.UsedItemWithArgument -= OnInventoryUsedItem;
				ItemUseSuccess -= Inventory.ItemUsed;
			}
			if (SMachine is PlayerStateMachine machine)
			{

				if (Equip != null)
				{
					Equip.WeaponChanged -= machine.OnWeaponChanged;
					Equip.OffhandChanged -= machine.OnOffhandChanged;
				}
				AttunementChanged -= machine.OnAttunementChanged;

				
				if (staggerComp != null)
					ActorGotStaggered -= SMachine.OnStagger;

				if (Stam != null)
					Stam.StaminaChanged -= machine.OnStaminaChanged;
			}
		}

		public void VisitedCheckPoint(Checkpoint checkpoint)
		{
			Resting = true;
			HP.Heal(HP.MaxValue);
			Equip.SheatheWeapon(CurrentWeapon);
			Equip.SheatheWeapon(CurrentOffhand);
			lastVisitedCheckpoint = checkpoint;
			SMachine.TransitionTo(nameof(PlayerRestingState), null);
			EmitSignal(SignalName.PlayerResting);
		}

		public void LeaveCheckpoint()
		{
			if (GetParent() is Map map)
			{
				map.OnCheckPointLeft();
			}
			Resting = false;
			lastVisitedCheckpoint.Visiting = false;
			(Animation as PlayerAnimation).Resting = false;
		}

		public void Respawn()
		{
			Camera.lockOnComponent.Targetted(null);
			Equip.SheatheWeapon(CurrentWeapon);
			Equip.SheatheWeapon(CurrentOffhand);
			Camera.Rotation = Rotation;
			SMachine.TransitionTo(nameof(PlayerRunState), null);
            SMachine.SetProcess(true);
            SMachine.SetPhysicsProcess(true);
            SetPhysicsProcess(true);
            SetProcess(true);
            HP.Heal(HP.MaxValue);
            Dead = false;
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
				AttunementIterator += 1;
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
			//(Animation as PlayerAnimation).CurrentMovementState = "Sprint";
			// Movement._Sprinting = true;
			// Movement.SetSpeed(Movement.SprintSpeed);
			// Stam.Regen = false;
			// Stam.Degen = true;
		}

		public void ReleaseSprint(bool changeSpeed = true)
		{
			if (Movement._Sprinting == false) return;
			(Animation as PlayerAnimation).CurrentMovementState = "Run";
			Movement._Sprinting = false;
			if (changeSpeed)
				Movement.SetSpeed(Movement.Speed);
			Stam.Regen = true;
			Stam.Degen = false;
		}

		#region Block

		public void Block()
		{
			_isBlocking = true;
		}

		public void ResetBlockedAttack()
		{
			_attackBlocked = false;
		}

		public void BlockedAttack(float damageToTake)
		{
			(Animation as PlayerAnimation).BlockedAttack = true;
			(Animation as PlayerAnimation).RequestOneShot("Offhand");
			audio.Play(SoundEffects.ShieldBlock);
			_attackBlocked = true;
			Stam.DecreaseStamina(damageToTake);
		}

		public void BlockCounterAttack()
		{
			_CanRotate = false;
			_IsAttacking = true;
			
			(Animation as PlayerAnimation).MainAttack(Animations.CounterLightAttack);
			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
			_attackBlocked = false;
		}

		public void BlockHold()
		{
			(Animation as PlayerAnimation).CurrentMovementState = "Walk";
			(Animation as PlayerAnimation).BlockedAttack = false;

			_isBlocking = true;
			_attackBlocked = false;
		}

		public void BlockRelease()
		{
			_attackBlocked = false;
			_isBlocking = false;
			//Stam.Degen = false;
			//Stam.Regen = true;
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
			(Animation as PlayerAnimation).MainAttack("Attack1");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack1);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
		}

		public void Attack2()
		{
			_IsAttacking = true;
			(Animation as PlayerAnimation).MainAttack("Attack2");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack2);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
		}

		public void Attack3()
		{
			_IsAttacking = true;
			(Animation as PlayerAnimation).MainAttack("Attack3");

			audio.Play(CurrentAttunement + CurrentWeapon.Name + Animations.Attack3);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
		}

		public void ComboAttack()
		{

		}

		public void SprintLightAttack()
		{
			(Animation as PlayerAnimation).MainAttack(Animations.SprintLightAttack);
			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
			_CanRotate = false;
			_IsAttacking = true;
		}

		public void SprintHeavyAttack()
		{
		}

		public void DodgeLightAttack()
		{
			(Animation as PlayerAnimation).MainAttack(Animations.DodgeLightAttack);
			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
			_CanRotate = false;
			_IsAttacking = true;
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
			(Animation as PlayerAnimation).RequestOneShot("Dodge");
			// if (HasWeapon())
			// 	Animation.Transition(CurrentWeapon.Name + Animations.TransitionMovement, CurrentWeapon.Name + Animations.Dodge);
			// else Animation.Transition(Animations.TransitionMovement, Animations.Dodge);
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

