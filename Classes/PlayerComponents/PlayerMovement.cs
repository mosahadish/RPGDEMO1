using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class PlayerMovement : Movement
	{

		private Vector2 inputDir;
		[Export] private float controllerDeadzone = 0.15f;
		[Export] Player player;

		private bool sprintTimerToggle = false;

		private float sprintTimer = 0.0f;


		public override void _PhysicsProcess(double delta)
		{
			inputDir = Input.GetVector("move_right", "move_left", "move_backwards", "move_forward", controllerDeadzone);
			
			WantsMovement(inputDir);

			DodgeAndSprint(inputDir);

			//Handle Jump
			if (Input.IsActionJustPressed(Actions.Jump)) WantsJump(inputDir);
		}



		private void DodgeAndSprint(Vector2 inputDir)
		{
			/*
				Dodge is executed once the "dodge" button is released
				When "dodge" button is first pressed, toggle sprint timer on
				This will start a "timer" that measures how much passed between pressed and releasing the "dodge" button
				And will dodge or sprint accordingly
			*/
			// if (Input.IsActionJustPressed(Actions.Dodge)) sprintTimerToggle = true;

			if (Input.IsActionPressed(Actions.Dodge))
			{
				sprintTimerToggle = true;

				if (Sprinting == false)
				{
					if (sprintTimer > 10) WantsSprint();
				}
			}

			else if (Input.IsActionJustReleased(Actions.Dodge))
			{
				sprintTimerToggle = false;
				if (sprintTimer < 10) WantsDodge(inputDir);
				if (Sprinting == true) WantsReleaseSprint();
			}

			if (sprintTimerToggle == true) sprintTimer += 1;
			else sprintTimer = 0;
		}

		public void HandleFreeMovement(Vector3 direction, double delta)
		{	
			if (direction == Vector3.Zero) return;
			//Tween tween = CreateTween();

			//direction = direction.Normalized();
			//player.LookAt(player.Position - direction, Vector3.Up);
			player.LookInDirection(direction);
			//tween.TweenMethod(Callable.From((Vector3 target) => LookAt(target, Vector3.Up)), player.Position, (player.Position - direction), 0.01 * delta);
			HandleMovement(direction, delta);
		}
	}
}
