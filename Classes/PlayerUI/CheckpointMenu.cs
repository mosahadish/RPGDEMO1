using Godot;
using System;

namespace Game
{
	[GlobalClass]
	public partial class CheckpointMenu : Control
	{
		[Signal] public delegate void PlayerFinishedRestingEventHandler();
		[Export] private AnimationPlayer anim;
 		[Export] private Button leaveBtn;
		[Export] private Button restBtn;
		
		private Color newMod;

		public override void _Ready()
        {
            base._Ready();
			newMod = Modulate;
			newMod.A = 0;

			Modulate = newMod;
        }

		//this needs to be connected to the PlayerResting signal in the Player, should be done my OnScreen.cs
		public void PlayerResting()
		{
			GD.Print("resting fade in ");
			anim.Play("FadeIn");
			restBtn.GrabFocus();
		}

		public async void PlayerStoppedResting()
		{
			anim.Play("FadeOut");
			await ToSignal(GetTree().CreateTimer(0.5), SceneTreeTimer.SignalName.Timeout);
			
			EmitSignal(SignalName.PlayerFinishedResting);
		}

		private void OnLeavePressed()
		{
			if (restBtn.HasFocus()) restBtn.ReleaseFocus();
			if (leaveBtn.HasFocus()) leaveBtn.ReleaseFocus();
			PlayerStoppedResting();
		}
	}
}
