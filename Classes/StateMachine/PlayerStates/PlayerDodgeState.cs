using Godot;
using System;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerDodgeState : State
    {

        private Vector3 direction;
        private CameraComponent camera;

        // public override void _Ready()
        // {
        //     base._Ready();
        //     AnimTransition = Animations.TransitionMovement;
        //     Anim = Animations.Dodge;
        // }

        public override void Enter(Dictionary<string, Vector2> msg)
        {   
            if (msg.ContainsKey(Actions.Dodge)) InputDir = msg[Actions.Dodge];
            if (InputDir == Vector2.Zero) InputDir = Vector2.Down;

            // Stam.DecreaseStamina(Stam.DodgeConsumption);
            // Stam.Regen = false;
            // Movement.CurrentSpeed = Movement.DodgeSpeed;
            // Actor._IsDodging = true;
            // Actor._CanRotate = false;

            direction = new Vector3(InputDir.X, 0, InputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y);
            (Actor as Player).Dodge();
            //SetAnim(Animations.TransitionMovement, Animations.Dodge);
           // Animation.Transition(AnimTransition, Anim);
        }

        public override void PhysicsUpdate(double delta)
        {
            Movement.HandleMovement(direction, delta);
        }

        public override void Update(double delta)
        {
        }

        public void SetCamera(CameraComponent camera)
		{
			this.camera = camera;
		}

        public override void Exit()
        {
            (Actor as Player).FinishDodging();
            // Movement.CurrentSpeed = Movement.Speed;
            // Stam.Regen = true;
            // Actor._IsDodging = false;
            // Actor._CanRotate = true;
        }
    }

}