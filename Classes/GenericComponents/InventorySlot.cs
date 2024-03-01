using Godot;
using Globals;
using System;

namespace Game
{
    [GlobalClass]
    public partial class InventorySlot : Button
    {
        [Export] Label quantityLabel;

        public Item item = null;

        private Item itemToMove = null;
        private string itemInfo;
        private bool empty = true;

        [Export(PropertyHint.Enum, Slots.Passive + ","+Slots.Head+","+ Slots.Body + "," + 
        Slots.Legs + "," + Slots.RightHand +"," + Slots.LeftHand)]
		public string Type {get; set;} = Slots.Passive;

        public override void _Ready()
        {
            base._Ready();
            if (Type == Slots.Passive) return;
            Text = Type;
        }


        public void UpdateQuantity()
        {
            //item.Quantity += addedQuant;
            if (item.Quantity == 0) RemoveItem();
        }

        public void EquipItem(InventorySlot fromSlot)
        {
            if (fromSlot.IsEmpty() || fromSlot.Type == Type) return;

            itemToMove = fromSlot.item;
            if (IsEmpty() == false)
            {
                fromSlot.AddItem(item);
            }
            else
            {
                fromSlot.RemoveItem();
            }
            AddItem(itemToMove);
        }

        public void MoveItem(InventorySlot toSlot)
        {
            if (toSlot.Type == Slots.Passive || toSlot.Type == item.Type) 
            {
                if (toSlot.IsEmpty())
                {
                    toSlot.AddItem(item);
                    RemoveItem();
                }
                else
                {
                    if (Type == Slots.Passive)
                    {
                        itemToMove = item;
                        AddItem(toSlot.item);
                        toSlot.AddItem(itemToMove);
                    }
                }
            }
        }

        public void AddItem(Item itemToAdd)
        {
            itemInfo = itemToAdd.Name + "\n";
            Icon = (Texture2D)itemToAdd.Texture;
            item = itemToAdd;

            quantityLabel.Text = "";
            empty = false;

            if (item is Weapon) itemInfo += "Damage: " + (item as Weapon).Damage.ToString() + "\n";
            
            itemInfo += item.HoverText;
            TooltipText = itemInfo;
        }

        public void RemoveItem()
        {
            Icon = null;
            item = null;
            quantityLabel.Text = "";
            empty = true;
            TooltipText = "";

            if (Type != Slots.Passive) Text = Type;
        }

        public bool IsEmpty()
        {
            return empty;
        }
    }
}