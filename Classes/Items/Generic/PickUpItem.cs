using Godot;

namespace Game
{
    [GlobalClass]
    public partial class PickUpItem : InteractableObject
    {
        [Export] PackedScene ItemScene = null;

        private bool PickedUp = false;

        public override void _Ready()
        {
            SetProcess(false);
        }

        public override void _Process(double delta)
        {
            if (PickedUp == true) QueueFree();
        }

        public Item GetItem()
        {
            if (ItemScene == null) return null;

            PickedUp = true;
            SetProcess(true);
            return (Item)ItemScene.Instantiate();
        }

        public override void OnInteract()
        {
            
        }
    }
}