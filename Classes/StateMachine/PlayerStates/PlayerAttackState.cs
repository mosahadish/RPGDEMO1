using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAttackState : State
    {
        private Vector2 inputDir;
        private Vector3 Direction;
        private int chain = 1;

        private CameraComponent camera;
    
        public override void _Ready()
        {
            base._Ready();
        }

        public override void Enter(Dictionary<string, Vector2> Msg)
        {
            if (Msg.ContainsKey(Actions.AttackLight))
            {
                if (Movement.Sprinting == true)
                {
                    (Actor as Player).SprintLightAttack();
                }
                else
                {
                    chain = (int)Msg[Actions.AttackLight].Y;

                    if (chain == 1) (Actor as Player).Attack1();
                    if (chain == 2) (Actor as Player).Attack2();
                    if (chain ==3) (Actor as Player).Attack3();
                }
            }

            if (camera.Target != null) Direction = Actor.GlobalPosition.DirectionTo(camera.Target.GlobalPosition);
            // global_transform.basis.z instaed of PositiveZ marker? NOT WORKING
            else if (InputDir == Vector2.Zero) Direction = ((Actor as Player).PositiveZ.GlobalPosition - Actor.GlobalPosition).Normalized();
            else 
            {
                Direction = new Vector3(InputDir.X, 0, InputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y).Normalized();
                InputDir = Vector2.Zero;
            }
        }


        public override void PhysicsUpdate(double delta)
        {
            Actor.LookInDirection(Direction);
            Movement.HandleMovement(Direction, delta);
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
            (Actor as Player).FinishAttacking();
        }
    }
}