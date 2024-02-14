using System.Collections.Generic;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Stamina : Node
    {
        public event NotifyValueChange StaminaChanged;

        [ExportCategory("StaminaStats")]
        [Export] public float MaxValue;
        [Export] public float DodgeConsumption;
        [Export] private float RegenPerSec;
        [Export] private float DegenPerSec;
        
        
        // [ExportCategory("Dependencies")]
        // [Export] StateMachine SMachine;

        public bool Regen = true;
        public bool Degen = false;
        private float currentValue = 0;
        private float regenRate = 0;
        private float degenRate = 0;

        public override void _Ready()
        {
            regenRate = RegenPerSec/60;
            degenRate = DegenPerSec/60;
            IncreaseStamina(MaxValue);
        }

        public override void _PhysicsProcess(double delta)
        {
            if (Regen == true)
            {
                if (currentValue < MaxValue)
                    IncreaseStamina(regenRate);
            }

            if (Degen == true)
            {
                DecreaseStamina(degenRate);
            }
        }

        public void IncreaseStamina(float value)
        {
            currentValue += value;
            if (currentValue > MaxValue) currentValue = MaxValue;

            StaminaChanged?.Invoke(currentValue);
        }

        public void DecreaseStamina(float value)
        {
            currentValue -= value;
            if (currentValue < 0) currentValue = 0;

            StaminaChanged?.Invoke(currentValue);
        }

        public bool HasEnough(float value)
        {
            return currentValue >= value;
        }
    }
}
