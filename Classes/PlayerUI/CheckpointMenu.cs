using Godot;
using System;

namespace Game
{
	[GlobalClass]
	public partial class CheckpointMenu : Control
	{
		[Signal] public delegate void PlayerFinishedRestingEventHandler();
		private AnimationPlayer anim;
 		private Button leaveBtn;
		private Button restBtn;
		
		private Color newMod;

		public override void _Ready()
        {
            base._Ready();
			newMod = Modulate;
			newMod.A = 0;

			Modulate = newMod;

			leaveBtn = GetNode<Button>("VBoxContainer/Leave");
			restBtn = GetNode<Button>("VBoxContainer/Rest");
			anim = GetNode<AnimationPlayer>("AnimationPlayer");
        }

		//this needs to be connected to the PlayerResting signal in the Player, should be done by OnScreen.cs
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
			
			EmitSignal(SignalName.PlayerFinishedResting); //Connected by OnScreen.cs
		}

		private void OnLeavePressed()
		{
			if (restBtn.HasFocus()) restBtn.ReleaseFocus();
			if (leaveBtn.HasFocus()) leaveBtn.ReleaseFocus();
			PlayerStoppedResting();
		}
	}
}
