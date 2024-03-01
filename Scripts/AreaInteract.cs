using Godot;
using Globals;
using System.Collections.Generic;
using GameSettings;

namespace Game
{
    [GlobalClass]
    public partial class AreaInteract : Area3D
    {
        [Signal]
        public delegate void PickedUpItemWithArgumentEventHandler(Item i);

        [Export] Label label;


        private const float MaxDistance = 9999999;
        private PickUpItem item;
        private List<PickUpItem> itemsInRange = new();
        private PickUpItem itemToPickUp;
        private InteractableObject interactable;
        private string pickUpBtn;
        private float shortestDistToItem = MaxDistance;
        private float dist;

        public override void _Ready()
        {
            pickUpBtn = Settings.GetBindForAction(Actions.PickUp);

            SetProcess(false);
        }

        public override void _Process(double delta)
        {
            FindClosestItem();

            if (Input.IsActionJustPressed(Actions.PickUp))
            {
                if (itemToPickUp != null)
                {
                    EmitSignal(SignalName.PickedUpItemWithArgument, item.GetItem());
                }
                else
                {
                    interactable?.OnInteract();
                }
            }

            if (itemToPickUp != null)
            {
                label.Text = pickUpBtn + ": Pick Up Item";
                label.Show();
            }
            else
            {
                label.Hide();
            }
        }

        private void FindClosestItem()
        {
            shortestDistToItem = MaxDistance;

            foreach (PickUpItem itm in itemsInRange)
            {
                dist = Position.DistanceSquaredTo(itm.Position);
                if (dist < shortestDistToItem)
                {
                    shortestDistToItem = dist;
                    itemToPickUp = itm;
                }
            }
        }

        private void OnAreaEntered(Area3D area)
        {
            SetProcess(true);
            interactable = (InteractableObject)area;

            if (interactable is PickUpItem)
            {
                interactable = null;
                item = (PickUpItem)area;
                itemsInRange.Add(item);
            }

        }

        private void OnAreaExited(Area3D area)
        {
            //item = (PickUpItem)area.GetParent();
            interactable = (InteractableObject)area;

            if (interactable is PickUpItem)
            {
                item = (PickUpItem)area;
                if (itemsInRange.Contains(item))
                {
                    itemsInRange.Remove(item);
                    if (item == itemToPickUp) itemToPickUp = null;
                }

            }

            interactable = null;

            if (itemsInRange.Count == 0)
            {
                SetProcess(false);
                label.Hide();
                label.Text = "";
            }

        }
    }
}