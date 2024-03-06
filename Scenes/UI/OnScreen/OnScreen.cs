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
		[Export] private CheckpointMenu checkPointMenu;
		[Export] private YouDied youDied;
		[Export] private TextureRect blackScreen;
		[Export] private TextureRect checkPointRect;
		[Export] private Hotbar hotbar;
		private Vector2 screenCenter;
		private Vector2 viewPortSize;
		
	// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			attun.Text = player.CurrentAttunement;

			stam.StaminaChanged += OnStaminaChanged;
			stamBar.MaxValue = stam.MaxValue;
			stamBar.Value = stam.MaxValue;

			health.ValueChangedWithArgument += OnHealthChanged;
			healthBar.MaxValue = health.MaxValue;
			healthBar.Value = health.MaxValue;

			if (player != null)
			{
				player.AttunementChanged += OnAttunementChanged;
				player.ActorDeathWithArgument += OnPlayerDeath;
				player.PlayerResting += checkPointMenu.PlayerResting;

				if (player.Camera != null)
				{
					player.Camera.AimWithArgument += OnAim;
				}
			}
			
			checkPointMenu.PlayerFinishedResting += player.LeaveCheckpoint;

			if (inventory != null && hotbar != null)
			{
				hotbar.HotbarInventory = inventory.hotbar;
				hotbar.HotbarUsedWithArgument += inventory.OnHotBarItemUsed;
			}

			GetViewport().SizeChanged += OnWindowSizeChanged;
		}

        private void OnPlayerDeath(Actor actor)
        {
            youDied.FadeIn();
        }

		private void OnAim(bool aiming)
		{
			aim.Visible = aiming;
		}

        private void OnWindowSizeChanged()
        {
			viewPortSize = GetViewportRect().Size;
            screenCenter.X = viewPortSize.X/2;
			screenCenter.Y = viewPortSize.Y/2;

			aim.GlobalPosition = screenCenter;
			inventory.GlobalPosition = screenCenter;
			interact.GlobalPosition = screenCenter;
			youDied.GlobalPosition = screenCenter;
			
			checkPointMenu.GlobalPosition = Vector2.Zero;
			blackScreen.GlobalPosition = Vector2.Zero;

			blackScreen.Size = viewPortSize;
			checkPointRect.Size = viewPortSize;
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
