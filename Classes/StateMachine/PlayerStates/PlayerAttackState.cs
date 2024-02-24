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

        private Player player;

        private CameraComponent camera;
    
        public override void _Ready()
        {
            base._Ready();
            //await ToSignal(Owner, "ready");
            player = Actor as Player; //why doesn't this work
        }

        public override void Enter(Dictionary<string, Vector2> Msg)
        {
            player ??= (Actor as Player);
           
            CalculateDirToFace();

            if (Msg.ContainsKey(Actions.DodgeAttackLight))
            {
                player.DodgeLightAttack();
            }

            if (Msg.ContainsKey(Actions.AttackLight))
            {
                if (Movement._Sprinting == true)
                {
                    player.SprintLightAttack();
                }
                else if (player.CanCounter())
                {
                    player.BlockCounterAttack();
                }
                else
                {
                    chain = (int)Msg[Actions.AttackLight].Y;

                    if (chain == 1) player.Attack1();
                    if (chain == 2) player.Attack2();
                    if (chain ==3) player.Attack3();
                }
            }
        }


        public override void PhysicsUpdate(double delta)
        {
            if (player._CanRotate) CalculateDirToFace();

            Actor.LookInDirection(Direction, true);
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
            player.FinishAttacking();
            //InputDir = Vector2.Zero;
        }

        private void CalculateDirToFace()
        {
            if (camera.Target != null) Direction = Actor.GlobalPosition.DirectionTo(camera.Target.GlobalPosition);
                // global_transform.basis.z instaed of PositiveZ marker? NOT WORKING
            else if (InputDir == Vector2.Zero) Direction = (player.PositiveZ.GlobalPosition - Actor.GlobalPosition).Normalized();
            else 
            {
                Direction = new Vector3(InputDir.X, 0, InputDir.Y).Rotated(Vector3.Up, camera.Rotation.Y).Normalized();
                InputDir = Vector2.Zero;
            } 
        }
    }
}