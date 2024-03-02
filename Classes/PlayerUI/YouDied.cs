using Godot;
using System;

namespace Game
{
	[GlobalClass]
	public partial class YouDied : Control
	{
		[Export] private AnimationPlayer anim;

		private Color newMod;

        public override void _Ready()
        {
            base._Ready();
			newMod = Modulate;
			newMod.A = 0;

			Modulate = newMod;
        }
        public void FadeIn()
		{
			anim.Play("FadeIn");
		}

		public void FadeOut()
		{
			anim.Play("FadeOut");
		}

		public void AnimationFinished(string anim)
		{
			if (anim == "FadeIn") FadeOut();
		}
	}

}