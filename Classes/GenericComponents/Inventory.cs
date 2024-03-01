using System;
using System.Collections.Generic;
using System.Linq;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Inventory: Control
    {
         [Signal]
        public delegate void EquippedItemWithArgumentEventHandler(Item item);
        [Signal]
        public delegate void UnequippedItemWithArgumentEventHandler(Item item);
        [Signal]
        public delegate void ChangedWeaponWithArgumentEventHandler(Weapon item);
        [Signal]
        public delegate void UsedItemWithArgumentEventHandler(Item item);

        [Export] Player player;
        [Export] private GridContainer inventory;
        [Export] private GridContainer equipment;
        [Export] private GridContainer hotbar;

        [ExportCategory("Equipment Slots")]
        [Export] private InventorySlot headSlot;
        [Export] private InventorySlot bodySlot;
        [Export] private InventorySlot legsSlot;
        [Export] private InventorySlot rightHandSlot;
        [Export] private InventorySlot leftHandSlot;

        private Dictionary<string, InventorySlot> currentEquip = new();


        private InventorySlot focusedSlot;
        private InventorySlot fromSlot;
        private InventorySlot toSlot;
        private InventorySlot lastUsedSlot;
        private bool dragging = false;
        private Vector2 mousePos;

        public async override void _Ready()
        {
            base._Ready();
            currentEquip[Slots.Head] = headSlot;
            currentEquip[Slots.Body] = bodySlot;
            currentEquip[Slots.Legs] = legsSlot;
            currentEquip[Slots.RightHand] = rightHandSlot;
            currentEquip[Slots.LeftHand] = leftHandSlot;

            await ToSignal(Owner, "ready");

            AddItem(GameData.Instance.FetchItem("HealPotion"));

            Input.MouseMode = Input.MouseModeEnum.Hidden;
            SetProcess(false);
            Hide();
        }

        public override void _Process(double delta)
        {
            DragAndDrop();

            if (player.IsDodging()) return;

            if (Input.IsActionJustPressed("ui_accept"))
            {
                if (focusedSlot.IsEmpty()) return;

                if (currentEquip.ContainsValue(focusedSlot))
                {
                    focusedSlot.MoveItem(FindFirstEmptySlot());
                }
                else if (IsEquippable(focusedSlot.item))
                {
                    //Get the corresponding equipment slot
                    toSlot = currentEquip[focusedSlot.item.Type];
                    //If the equipment slot is not empty, remove the currently equipped item on the player
                    if (toSlot.IsEmpty() == false) EmitSignal(SignalName.UnequippedItemWithArgument, toSlot.item);

                    //Equip the item
                    toSlot.EquipItem(focusedSlot);
                    EmitSignal(SignalName.EquippedItemWithArgument, toSlot.item);
                }
                else if (focusedSlot.item is Consumable)
                {
                    lastUsedSlot = focusedSlot;
                    EmitSignal(SignalName.UsedItemWithArgument, focusedSlot.item);
                }
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (player.Resting)
            {
                return;
            }

            base._UnhandledInput(@event);
            if (@event is InputEventJoypadButton)
            {
                if (InputMap.ActionHasEvent("open_inventory", @event))
                {
                    if (@event.IsPressed())
                    {
                        if (Visible == false) Open();
                        else Close();
                    }
                }

                else if (InputMap.ActionHasEvent("DrawRightWeapon", @event))
                {
                    if (@event.IsPressed())
                    {
                        if (currentEquip[Slots.RightHand].IsEmpty() == false) 
                        {
                            EmitSignal(SignalName.ChangedWeaponWithArgument,currentEquip[Slots.RightHand].item);
                        }
                    }
                }
                else if (InputMap.ActionHasEvent("DrawLeftWeapon", @event))
                {
                    if (@event.IsPressed())
                    {
                        if (currentEquip[Slots.LeftHand].IsEmpty() == false) 
                        {
                            EmitSignal(SignalName.ChangedWeaponWithArgument,currentEquip[Slots.LeftHand].item);
                        }
                    }
                }
            }
        }


        public static bool IsEquippable(Item item)
        {
            if (item is Weapon) return true;
            return false;
        }

        public void AddItem(Item itemToAdd)
        {
            FindFirstEmptySlot().AddItem(itemToAdd);
        }

        private InventorySlot FindFirstEmptySlot()
        {
            foreach (InventorySlot slot in inventory.GetChildren())
            {
                if (slot.IsEmpty()) return slot;
            }

            return null;
        }

        private void DragAndDrop()
        {   
            if (Input.IsActionJustPressed("ui_select"))
            {
                if (dragging == false) Drag();
                else Drop();
            }

            else if (Input.IsActionJustPressed("ui_cancel")) CancelDrag();
        }

        private void Drag()
        {
            if (focusedSlot.IsEmpty()) return;
            
            fromSlot = focusedSlot;
            fromSlot.Disabled = true;
            dragging = true;
        }

        private void CancelDrag()
        {
            if (dragging == false) return;

            fromSlot.Disabled = false;
            dragging = false;
        }

        private void Drop()
        {
            toSlot = focusedSlot;
            fromSlot.Disabled = false;
            fromSlot.MoveItem(toSlot);
            dragging = false;
        }

        private void Open()
        {
            //if (GetViewport().IsConnected("GuiFocusChanged", Callable.From(() => OnFocusedChanged(focusedSlot))) == false)
            GetViewport().GuiFocusChanged += OnFocusedChanged;
            player.CanInteract = false;

            SetProcess(true);
            Show();
            ((Control)inventory.GetChild(0)).GrabFocus();
            WarpMouse(mousePos);
        }

        private void Close()
        {
            GetViewport().GuiFocusChanged -= OnFocusedChanged;
            player.CanInteract = true;

            SetProcess(false);
            Hide();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            ClearInventory();
        }

        private void ClearInventory()
        {
            GD.Print("Freeing inventory");
            foreach (InventorySlot slot in inventory.GetChildren().Cast<InventorySlot>())
            {
                if (slot.IsEmpty() == false) slot.item.QueueFree(); 
            }
            foreach (InventorySlot slot in equipment.GetChildren().Cast<InventorySlot>())
            {
                if (slot.IsEmpty() == false) slot.item.QueueFree(); 
            }
            foreach (InventorySlot slot in hotbar.GetChildren().Cast<InventorySlot>())
            {
                if (slot.IsEmpty() == false) slot.item.QueueFree(); 
            }
        }

        private void OnFocusedChanged(Control node)
        {
            if (node == null) return;

            focusedSlot = (InventorySlot) node;
            mousePos = ((Control)(focusedSlot.GetParent())).Position;
            mousePos.X += 40;
            mousePos.Y -= 10;

            mousePos += focusedSlot.Position;
        }

        internal void ItemUsed()
        {
            lastUsedSlot.UpdateQuantity();
        }
    }
}