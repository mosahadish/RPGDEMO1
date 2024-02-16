using Godot;
using Globals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
	public delegate void NotifyValueChange(float value);
	public delegate void NotifyAttunement(string msg);
	public delegate void NotifyWeapon(Weapon weapon);
	[GlobalClass]
	public partial class Player : Actor, IBlocker, IMeleeAttacker, IDodger
	{

		public event NotifyAttunement AttunementChanged;
		public event NotifyWeapon WeaponChanged;
		public event NotifyWeapon OffhandChanged;

		[Export(PropertyHint.Enum, Attunements.Lightning+
		 "," + Attunements.Fire)]
		public string CurrentAttunement {get; set;} = Attunements.Fire;

		[ExportCategory("Equipment Attachments")]
		[Export] private BoneAttachment3D RightHand;
		[Export] private BoneAttachment3D LeftHand;
		[Export] private BoneAttachment3D RightEquip;
		[Export] private BoneAttachment3D LeftEquip;
		[Export] private BoneAttachment3D BackEquip;
		// [Export] string CurrentAttunement;

		[ExportCategory("Dependencies")]
		[Export] public CameraComponent Camera;

		[ExportCategory("Help")]
		[Export] public Marker3D PositiveZ;
		[Export] private Aim aim;

		private Dictionary<Item, Node3D> EquippedItems = new();
		private int AttunementIterator = 0;
		private string[] arrAttunement = {Attunements.Fire, Attunements.Lightning};

		// string CurrentAttunement = Attunements.Fire;

		public override void _Ready()
		{
			AttunementChanged.Invoke(CurrentAttunement);
		}

		public override void _Process(double delta)
		{
			if (Input.IsActionJustPressed(Actions.ChangeAttunement) && _IsAttacking == false && Attack.ReadyToShoot == false)
			{
				AttunementIterator +=1;
				if (AttunementIterator == arrAttunement.Length) AttunementIterator = 0;
				CurrentAttunement = arrAttunement[AttunementIterator];

				AttunementChanged?.Invoke(CurrentAttunement);
			}
			
		}

		private void OnInventoryChangedWeapon(Weapon weap)
		{
			if (_IsDodging || _IsBlocking || _IsAttacking) return;
			if (HasWeapon())
			{
				if (CurrentWeapon == weap)
				{
					SheatheWeapon(CurrentWeapon);
					return;	
				}

				if (CurrentWeapon.SubType.Contains("2H"))
				{
					SheatheWeapon(CurrentWeapon);
				}
			}

			if (HasOffhand())
			{
				if (CurrentOffhand == weap)
				{
					SheatheWeapon(CurrentOffhand);
					return;
				}
			}

			if (weap.SubType.Contains("2H"))
			{
				SheatheWeapon(CurrentOffhand);
				SheatheWeapon(CurrentWeapon);
			}

			else if (weap.SubType == WeaponTypes.Melee1H)
			{
				SheatheWeapon(CurrentWeapon);
			}

			else if (weap.SubType == WeaponTypes.Offhand)
			{
				SheatheWeapon(CurrentOffhand);
			}

			DrawWeapon(weap);
		}

		private void SheatheWeapon(Weapon weap)
		{
			if (weap == null) return;
			if (EquippedItems.ContainsKey(weap))
			{
				Node3D CurrentAttach = EquippedItems[weap];
				GD.Print(CurrentAttach);
				CurrentAttach.RemoveChild(weap);

				weap.Wielder = null;
				
				if (weap == CurrentWeapon)
				{
					CurrentWeapon = null;
					WeaponChanged?.Invoke(CurrentWeapon);
				}
				else
				{
					CurrentOffhand = null;
					OffhandChanged?.Invoke(CurrentOffhand);
				}

				EquipWeapon(weap);
			}
		}

		private void DrawWeapon(Weapon weap)
		{
			if (EquippedItems.ContainsKey(weap))
			{
				// GD.Print(weap.Name);
				//Remove from the Equip place
				Node3D CurrentAttach = EquippedItems[weap];
				CurrentAttach.RemoveChild(weap);

				//Equip according to Slot types
				if (weap.Type == Slots.RightHand)
				{
					EquippedItems[weap] = (Node3D)RightHand.GetNode(weap.Name);
					EquippedItems[weap].AddChild(weap);
				}

				if (weap.Type == Slots.LeftHand)
				{
					EquippedItems[weap] = (Node3D)LeftHand.GetNode(weap.Name);
					EquippedItems[weap].AddChild(weap);
				}

				weap.Wielder = this;
				
				if (weap.SubType == WeaponTypes.Offhand)
				{
					CurrentOffhand = weap;
					OffhandChanged?.Invoke(CurrentOffhand);
				}
				else
				{
					CurrentWeapon = weap;
					WeaponChanged?.Invoke(CurrentWeapon);
				}
			}
		}

		public void OnInventoryEquippedItem(Item item)
		{
			if (_IsDodging || _IsBlocking || _IsAttacking) return;
			if (item is Weapon) EquipWeapon(item as Weapon);
		}

		private void EquipWeapon(Weapon weap)
		{
			if (weap.Type == Slots.RightHand)
			{
				EquippedItems[weap] = (Node3D)RightEquip.GetNode(weap.Name);
				EquippedItems[weap].AddChild(weap);
			}

			if (weap.Type == Slots.LeftHand)
			{
				EquippedItems[weap] = (Node3D)BackEquip.GetNode(weap.Name);
				EquippedItems[weap].AddChild(weap);
			}
		}

		private void OnInventoryUnequippedItem(Item item)
		{
			if (item is Weapon) UnequipWeapon(item as Weapon);
		}

		private void UnequipWeapon(Weapon weap)
		{
			if (EquippedItems.ContainsKey(weap))
			{
				GD.Print(weap);
				//Remove from the Equip place
				Node3D CurrentAttach = EquippedItems[weap];
				CurrentAttach.RemoveChild(weap);

				if (weap == CurrentWeapon)
				{
					CurrentWeapon = null;

					WeaponChanged?.Invoke(CurrentWeapon);
				}

				if (weap == CurrentOffhand)
				{
					CurrentOffhand = null;

					OffhandChanged?.Invoke(CurrentOffhand);
				}
			}
			
		}

		public Vector3 GetAimPoint()
		{
			return aim.GetAimPoint();
		}

		public void Block()
        {
            Animation.Transition("Shield", Animations.Block);
			if (HasWeapon())
				Animation.Transition(CurrentWeapon.Name+Animations.TransitionMovement, CurrentWeapon.Name + Animations.Walk);
			else Animation.Transition(Animations.TransitionMovement, Animations.Walk);
			_IsBlocking = true;
			Stam.Degen = true;
			Stam.Regen = false;
        }

		public void BlockAttack()
        {
            Animation.Transition("Shield", Animations.BlockedAttack);
			_IsBlocking = false;
        }

        public void BlockHold()
        {
            Animation.Transition("Shield", Animations.BlockHold);
			_IsBlocking = true;
        }

        public void BlockRelease()
        {
			if (_IsBlocking == false) return;

			Animation.Transition("Shield", Animations.BlockRelease);
			if (HasWeapon())
				Animation.Transition(CurrentWeapon.Name+Animations.TransitionMovement, CurrentWeapon.Name+Animations.Movement);
			else Animation.Transition(Animations.TransitionMovement, Animations.Movement);
            
			_IsBlocking = false;
			Stam.Degen = false;
			Stam.Regen = true;
        }

        public bool IsBlocking()
        {
            return _IsBlocking;
        }

        public void Attack1()
        {
            _IsAttacking = true;
			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack1);
			Animation.OneShot(CurrentWeapon.Name);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
        }

        public void Attack2()
        {
            _IsAttacking = true;
			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack2);
			Animation.OneShot(CurrentWeapon.Name);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
        }

        public void Attack3()
        {
            _IsAttacking = true;
			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.Attack3);
			Animation.OneShot(CurrentWeapon.Name);

			Stam.DecreaseStamina(CurrentWeapon.LightAttackStamConsumption);
            Stam.Regen = false;
		}

		public void SprintLightAttack()
        {
            _IsAttacking = true;
			Movement.Sprinting = false;

			Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.SprintLightAttack);
			Animation.Transition(CurrentWeapon.Name + Animations.Movement, CurrentWeapon.Name+Animations.SprintLightAttack);
			Animation.OneShot(CurrentWeapon.Name);
        }

		public void SprintHeavyAttack()
        {
            // _IsAttacking = true;
			// Animation.Transition(CurrentWeapon.Name + CurrentAttunement, CurrentWeapon.Name+Animations.SprintAttack);
			// Animation.OneShot(CurrentWeapon.Name);
        }

        public void DodgeAttack()
        {
            
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
			Stam.Regen = true;
			Movement.SetSpeed(Movement.Speed);
		}

        public bool IsAttacking()
        {
            return _IsAttacking;
        }

        public void Dodge()
        {
			Stam.DecreaseStamina(Stam.DodgeConsumption);
            Stam.Regen = false;
            Movement.CurrentSpeed = Movement.DodgeSpeed;
            _IsDodging = true;
            _CanRotate = false;
			if (HasWeapon())
            	Animation.Transition(CurrentWeapon.Name + Animations.TransitionMovement, CurrentWeapon.Name + Animations.Dodge);
			else Animation.Transition(Animations.TransitionMovement, Animations.Dodge);
        }

		public void FinishDodging()
		{
			Movement.CurrentSpeed = Movement.Speed;
            Stam.Regen = true;
            _IsDodging = false;
            _CanRotate = true;
		}

        public bool IsDodging()
        {
            return _IsDodging;
        }
    }
}

