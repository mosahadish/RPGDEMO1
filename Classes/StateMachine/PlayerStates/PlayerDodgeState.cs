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

        public override void Enter(Dictionary<string, Vector2> msg)
        {   
            if (msg.ContainsKey(Actions.Dodge)) InputDir = msg[Actions.Dodge];
            if (InputDir == Vector2.Zero) InputDir = Vector2.Down;

            direction = new Vector3(InputDir.X, 0, InputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y).Normalized();
            
            (Actor as Player).Dodge();
        }

        public override void PhysicsUpdate(double delta)
        {
            Actor.LookInDirection(direction, true);
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
        }
    }

}