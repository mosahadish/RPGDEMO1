using System.Collections.Generic;
using System.Text;
using Globals;
using Godot;

namespace Game
{
    public delegate void NotifyStagger();
    [GlobalClass]
    public partial class Stagger : Node
    {
        
        public event NotifyStagger Staggered;

        [ExportCategory("Stats")]
        [Export] private float maxValue;
        [Export] private float staggerRegen;
        [Export] private float staggerResetTimer;

        private float currentValue;
        private bool regen = true;

        private double timer;


        public async override void _Ready()
        {
            timer = staggerResetTimer;
            currentValue = maxValue;
            
            await ToSignal(Owner, "ready");

            if (Owner is Player player)
            {
                if (player.SMachine != null)
                {
                    Staggered += player.SMachine.OnStagger;
                }
            }
            else if (Owner is AI ai)
            {
                if (ai.SMachine != null)
                {
                    Staggered += ai.SMachine.OnStagger;
                }
            }
            
        }

        public override void _PhysicsProcess(double delta)
        {
            if (regen && currentValue < maxValue)
            {
                currentValue += staggerRegen;
            }
            if (regen == false)
            {
                timer -= delta;
                if (timer <= 0 ) 
                {
                    timer = staggerResetTimer;
                    regen = true;
                }
            }
        }

        public void TakeDamage(float damageToTake)
        {
            regen = false;
            currentValue -= damageToTake;

            if (currentValue <= 0)
            {
                currentValue = maxValue;
                Staggered?.Invoke();
            }
        }
    }
}

