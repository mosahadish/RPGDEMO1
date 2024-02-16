using Godot;
using System;

namespace Game
{
	public partial class OnScreen : Control
	{
		[Export] private Stamina stam;
		[Export] private Health health;
		[Export] private ProgressBar stamBar;
		[Export] private ProgressBar healthBar;
	// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			stam.StaminaChanged += OnStaminaChanged;
			stamBar.MaxValue = stam.MaxValue;
			stamBar.Value = stam.MaxValue;

			health.ValueChangedWithArgument += OnHealthChanged;
			healthBar.MaxValue = health.MaxValue;
			healthBar.Value = health.MaxValue;
		}

        private void OnHealthChanged(float value)
        {
            healthBar.Value = value;
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
		{
		}

		private void OnStaminaChanged(float new_val)
		{
			stamBar.Value = new_val;
		}
	}	
}
