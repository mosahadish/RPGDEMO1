using Godot;
using System;

namespace Game
{
	public partial class OnScreen : Control
	{
		[Export] private Stamina stam;
		[Export] private ProgressBar stam_bar;
	// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			stam.StaminaChanged += OnStaminaChanged;
			stam_bar.MaxValue = stam.MaxValue;
			stam_bar.Value = stam.MaxValue;
		}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		private void OnStaminaChanged(float new_val)
		{
			stam_bar.Value = new_val;
		}
	}	
}
