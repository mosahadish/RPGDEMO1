using System.Collections.Generic;
using System.Text;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Health : Node
    {
        [Signal]
        public delegate void ValueChangedWithArgumentEventHandler(float val);
        [Signal]
        public delegate void ActorDiedEventHandler();

        [ExportCategory("Stats")]
        [Export] public float MaxValue;

        private float currentVal;

        private Actor parent;

        //[Export]
        private PackedScene takeDamageScene = (PackedScene)ResourceLoader.Load("res://Scenes/Components/damage_floater.tscn");
        private Node3D floaterInstance;

        public override void _Ready()
        {
            currentVal = MaxValue;
            if (Owner is Actor actor)
            {
                parent = actor;
                ActorDied += actor.OnDeath;
            }
        }

        public void SetMaxValue(float newVal)
        {
            MaxValue = newVal;
        }

        public float GetValue()
        {
            return currentVal;
        }

        public void Heal(float valueToHeal)
        {
            currentVal += valueToHeal;
            if (currentVal > MaxValue) currentVal = MaxValue;

            EmitSignal(SignalName.ValueChangedWithArgument, currentVal);
        }

        public void TakeDamage(float damageToTake)
        {
            currentVal -= damageToTake;
            if (currentVal <= 0) 
            {
                currentVal = 0;
                EmitSignal(SignalName.ActorDied);
            }

            floaterInstance = (Node3D)takeDamageScene.Instantiate();
            parent.AddChild(floaterInstance);

            floaterInstance.Call("display_damage", damageToTake, parent.LockOn.GlobalPosition);
            EmitSignal(SignalName.ValueChangedWithArgument, currentVal);
        }

        public float AsPercent()
        {
            return currentVal / MaxValue;
        }
    }
}