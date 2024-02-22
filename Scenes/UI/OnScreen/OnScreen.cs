using Godot;
using System;

namespace Game
{
	public partial class OnScreen : Control
	{
		[Export] private Player player;
		[Export] private Stamina stam;
		[Export] private Health health;
		[Export] private ProgressBar stamBar;
		[Export] private ProgressBar healthBar;
		[Export] private Label attun;
		[Export] private Label interact;
		[Export] private TextureRect aim;
		[Export] private Inventory inventory;
		private Vector2 screenCenter;
	// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			player.AttunementChanged += OnAttunementChanged;
			attun.Text = player.CurrentAttunement;

			stam.StaminaChanged += OnStaminaChanged;
			stamBar.MaxValue = stam.MaxValue;
			stamBar.Value = stam.MaxValue;

			health.ValueChangedWithArgument += OnHealthChanged;
			healthBar.MaxValue = health.MaxValue;
			healthBar.Value = health.MaxValue;

			GetViewport().SizeChanged += OnWindowSizeChanged;
		}

        private void OnWindowSizeChanged()
        {
            screenCenter.X = GetViewportRect().Size.X/2;
			screenCenter.Y = GetViewportRect().Size.Y/2;

			aim.GlobalPosition = screenCenter;
			inventory.GlobalPosition = screenCenter;
			interact.GlobalPosition = screenCenter;
        }


        private void OnAttunementChanged(string attun)
		{
			this.attun.Text = attun;
		}

        private void OnHealthChanged(float value)
        {
            healthBar.Value = value;
        }

		private void OnStaminaChanged(float new_val)
		{
			stamBar.Value = new_val;
		}
	}	
}
