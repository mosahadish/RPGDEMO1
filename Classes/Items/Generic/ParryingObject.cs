using Godot;

namespace Game
{
     [GlobalClass]
    public abstract partial class ParryingObject : Weapon
    {
        //public event NotifyValueChange ParryWindowOver;
        [Export] public double DefaultParryWindowInSec;
        [Export] public float StaminaConsumption;
        protected double parryWindow;
        protected bool parryActive;
        public bool IsActive()
        {
            return parryActive;
        }
        
        public override void _PhysicsProcess(double delta)
        {
            parryWindow -= delta;
        
            if (parryWindow <= 0)
            {
                DeactivateParryWindow();
            }
        }

        public void ActivateParryWindow()
        {
            if (parryActive) return;

            SetPhysicsProcess(true);
            parryActive = true;
        }

        public void DeactivateParryWindow()
        {
            SetPhysicsProcess(false);
            parryActive = false;
            parryWindow = DefaultParryWindowInSec;
        }
    }
}
