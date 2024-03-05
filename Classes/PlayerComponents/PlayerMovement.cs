using System;
using System.Collections.Generic;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class PlayerMovement : Movement
	{

		private Vector2 inputDir;
		[Export] private PlayerStateMachine sMachine;
		[Export] private float controllerDeadzone;
		[Export] private float lightningMultiplier;
		[Export] private float fireMultiplier;

		private bool sprintTimerToggle = false;
		private float sprintTimer = 0.0f;
		private float attunementMultiplier;
		private float usedSpeedValue;

        public override void _Ready()
        {
            base._Ready();
			(Actor as Player).AttunementChanged += OnAttunementChanged;
        }

        private void OnAttunementChanged(string msg)
        {
            if (msg == Attunements.Fire) attunementMultiplier = fireMultiplier;
			if (msg == Attunements.Lightning) attunementMultiplier = lightningMultiplier;
			
			SetSpeed(usedSpeedValue);
        }


        public override void _PhysicsProcess(double delta)
		{
			inputDir = Input.GetVector("move_right", "move_left", "move_backwards", "move_forward", controllerDeadzone);
			
			WantsMovement(inputDir);

			DodgeAndSprint(inputDir);

			//Handle Jump
			if (Input.IsActionJustPressed(Actions.Jump)) WantsJump(inputDir);
		}

		public override void SetSpeed(float speed)
		{
			usedSpeedValue = speed;
			CurrentSpeed = usedSpeedValue * attunementMultiplier;
		}

		private void DodgeAndSprint(Vector2 inputDir)
		{
			/*
				Dodge is executed once the "dodge" button is released
				When "dodge" button is first pressed, toggle sprint timer on
				This will start a "timer" that measures how much passed between pressed and releasing the "dodge" button
				And will dodge or sprint accordingly
			*/

			if (Input.IsActionPressed(Actions.Dodge))
			{
				sprintTimerToggle = true;

				if (_Sprinting == false)
				{
					if (sprintTimer > 10) WantsSprint();
				}
			}

			else if (Input.IsActionJustReleased(Actions.Dodge))
			{
				sprintTimerToggle = false;
				if (sprintTimer < 10) WantsDodge(inputDir);
				if (_Sprinting == true) WantsReleaseSprint();
			}

			if (sprintTimerToggle == true) sprintTimer += 1;
			else sprintTimer = 0;
		}

		public void HandleFreeMovement(Vector3 direction, double delta)
		{	
			if (direction == Vector3.Zero) return;
	
			Actor.LookInDirection(direction.Normalized());
			HandleMovement(direction, delta);
		}


		public void WantsMovement(Vector2 inputDir)
        {
            Dictionary<string, Vector2> msg = new()
            {
                { "Run", inputDir }
            };
            sMachine.HandleMovementInput(msg);
        }

        public void WantsDodge(Vector2 inputDir)
        {
            Dictionary<string, Vector2> msg = new()
            {
                { Actions.Dodge, inputDir }
            };
            sMachine.HandleMovementInput(msg);
        }

        public void WantsJump(Vector2 inputDir)
        {
            Dictionary<string, Vector2> msg = new()
            {
                { Actions.Jump, inputDir }
            };
             sMachine.HandleMovementInput(msg);
        }


        public void WantsSprint()
        {
            Dictionary<string, Vector2> msg = new()
            {
                { Actions.Sprint, inputDir}
            };
            sMachine.HandleMovementInput(msg);
        }

        public void WantsReleaseSprint()
        {
            Dictionary<string, Vector2> msg = new()
            {
                { Actions.SprintRelease, inputDir }
            };
            sMachine.HandleMovementInput(msg);
        }
	}
}
