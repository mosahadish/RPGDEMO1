using Godot;
using System;
using GameSettings;
using Globals;

namespace Game
{
	public partial class Tutorial : Control
	{
		[Export] Label instructionLabel;


		private string lightAttackBind;
		private string forwardBind;
		private string leftBind;
		private string rightBind;
		private string backBind;
		private string dodgeBind;

		public override void _Ready()
		{
			SetBindings();
			instructionLabel.Text = lightAttackBind;
		}


		public override void _Process(double delta)
		{
		}

		public void SetBindings()
		{
			lightAttackBind = Settings.GetBindForAction(Actions.AttackLight);
			forwardBind = Settings.GetBindForAction(Actions.MoveForward);
			leftBind = Settings.GetBindForAction(Actions.MoveLeft);
			rightBind = Settings.GetBindForAction(Actions.MoveRight);
			backBind = Settings.GetBindForAction(Actions.MoveBack);
			dodgeBind = Settings.GetBindForAction(Actions.Dodge);
		}
	}
}
