using Godot;
using System;
using System.Collections.Generic;
using Globals;

namespace Game
{
	public delegate void NotifyWeapon(Weapon weapon);
	[GlobalClass]
	public partial class PlayerEquipmentHandler : Node3D
	{
		/*
		This works directly with the player character and the Inventory system
		The player should inject itself and the Inventory to this in order for this to work
		*/
		public event NotifyWeapon WeaponChanged;
		public event NotifyWeapon OffhandChanged;

		[ExportCategory("Equipment Attachments")]
		[Export] private BoneAttachment3D RightHand;
		[Export] private BoneAttachment3D LeftHand;
		[Export] private BoneAttachment3D RightEquip;
		[Export] private BoneAttachment3D LeftEquip;
		[Export] private BoneAttachment3D BackEquip;

		private Dictionary<Item, Node3D> equippedItems = new();
		public AudioHandler Audio;
		public Inventory Inventory;
		public Player Player;
		// Called when the node enters the scene tree for the first time.
		public override async void _Ready()
		{
			await ToSignal(GetParent(), "ready");

			if (Player == null) QueueFree();
			if (Inventory == null) QueueFree();
			else 
			{
				Inventory.EquippedItemWithArgument += OnInventoryEquippedItem;
				Inventory.UnequippedItemWithArgument += OnInventoryUnequippedItem;
				Inventory.ChangedWeaponWithArgument += OnInventoryChangedWeapon;
			}
		}


        public override void _ExitTree()
        {
            base._ExitTree();
			if (Inventory != null)
			{
				Inventory.EquippedItemWithArgument -= OnInventoryEquippedItem;
				Inventory.UnequippedItemWithArgument -= OnInventoryUnequippedItem;
				Inventory.ChangedWeaponWithArgument -= OnInventoryChangedWeapon;
			}
        }

        public void OnInventoryChangedWeapon(Weapon weap)
		{
			if (Player.IsDodging() || Player.IsBlocking() || Player.IsAttacking()) return;
			if (Player.HasWeapon())
			{
				if (Player.CurrentWeapon == weap)
				{
					SheatheWeapon(Player.CurrentWeapon);
					return;	
				}

				if (Player.CurrentWeapon.SubType.Contains("2H"))
				{
					SheatheWeapon(Player.CurrentWeapon);
				}
			}

			if (Player.HasOffhand())
			{
				if (Player.CurrentOffhand == weap)
				{
					SheatheWeapon(Player.CurrentOffhand);
					return;
				}
			}

			if (weap.SubType.Contains("2H"))
			{
				SheatheWeapon(Player.CurrentOffhand);
				SheatheWeapon(Player.CurrentWeapon);
			}

			else if (weap.SubType == WeaponTypes.Melee1H)
			{
				SheatheWeapon(Player.CurrentWeapon);
			}

			else if (weap.SubType == WeaponTypes.Offhand)
			{
				SheatheWeapon(Player.CurrentOffhand);
			}

			DrawWeapon(weap);
		}

		private void SheatheWeapon(Weapon weap)
		{
			if (weap == null) return;
			if (equippedItems.ContainsKey(weap))
			{
				Node3D CurrentAttach = equippedItems[weap];
				CurrentAttach.RemoveChild(weap);

				weap.Wielder = null;
				
				if (weap == Player.CurrentWeapon)
				{
					Player.CurrentWeapon.NoAttunements();
					Player.CurrentWeapon = null;
					WeaponChanged?.Invoke(Player.CurrentWeapon);
				}
				else
				{
					Player.CurrentOffhand = null;
					OffhandChanged?.Invoke(Player.CurrentOffhand);
				}

				EquipWeapon(weap);
			}
		}

		private void DrawWeapon(Weapon weap)
		{
			if (equippedItems.ContainsKey(weap))
			{
				//Remove from the Equip place
				Node3D CurrentAttach = equippedItems[weap];
				CurrentAttach.RemoveChild(weap);

				//Equip according to Slot types
				if (weap.Type == Slots.RightHand)
				{
					equippedItems[weap] = (Node3D)RightHand.GetNode(weap.Name);
					equippedItems[weap].AddChild(weap);
				}

				if (weap.Type == Slots.LeftHand)
				{
					equippedItems[weap] = (Node3D)LeftHand.GetNode(weap.Name);
					equippedItems[weap].AddChild(weap);
				}

				weap.Wielder = Player;
				
				if (weap.SubType == WeaponTypes.Offhand)
				{
					Player.CurrentOffhand = weap;
					OffhandChanged?.Invoke(Player.CurrentOffhand);
				}
				else
				{
					Player.CurrentWeapon = weap;
					Player.CurrentWeapon.SetAttunement(Player.CurrentAttunement);
					WeaponChanged?.Invoke(Player.CurrentWeapon);
				}
			}
		}

		public void OnInventoryEquippedItem(Item item)
		{
			if (Player.IsDodging() || Player.IsBlocking() || Player.IsAttacking()) return;
			if (item is Weapon) EquipWeapon(item as Weapon);
		}

		private void EquipWeapon(Weapon weap)
		{
			if (weap.Type == Slots.RightHand)
			{
				equippedItems[weap] = (Node3D)RightEquip.GetNode(weap.Name);
				equippedItems[weap].AddChild(weap);
			}

			if (weap.Type == Slots.LeftHand)
			{
				equippedItems[weap] = (Node3D)BackEquip.GetNode(weap.Name);
				equippedItems[weap].AddChild(weap);
			}
			Audio.Play(SoundEffects.EquipItem);
		}

		public void OnInventoryUnequippedItem(Item item)
		{
			if (item is Weapon) UnequipWeapon(item as Weapon);
		}

		private void UnequipWeapon(Weapon weap)
		{
			if (equippedItems.ContainsKey(weap))
			{
				//Remove from the Equip place
				Node3D CurrentAttach = equippedItems[weap];
				CurrentAttach.RemoveChild(weap);

				if (weap == Player.CurrentWeapon)
				{
					Player.CurrentWeapon = null;

					WeaponChanged?.Invoke(Player.CurrentWeapon);
				}

				if (weap == Player.CurrentOffhand)
				{
					Player.CurrentOffhand = null;

					OffhandChanged?.Invoke(Player.CurrentOffhand);
				}
			}
			
		}

	}
}
