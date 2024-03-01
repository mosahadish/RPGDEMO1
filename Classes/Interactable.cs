using Godot;

namespace Game
{
    [GlobalClass]
    public abstract partial class InteractableObject : Area3D
    {
        public abstract void OnInteract();
    }
}