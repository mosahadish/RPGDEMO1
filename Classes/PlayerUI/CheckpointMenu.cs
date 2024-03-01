using Godot;
using System;

namespace Game
{
	public partial class CheckpointMenu : Control
	{
		[Signal] public delegate void PlayerFinishedRestingEventHandler();
		[Export] private AnimationPlayer anim;
 		[Export] private Button leaveBtn;
		[Export] private Button restBtn;
		

		public void PlayerResting()
		{
			anim.Play("FadeIn");
			restBtn.GrabFocus();
		}

		public void PlayerStoppedResting()
		{
			anim.Play("FadeOut");
			
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
