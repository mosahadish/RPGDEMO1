using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AIDodgeState : AIState
    {
        private Vector3 dirToDodge;

        public override void Enter(Player target)
        {   
            dirToDodge = target.GlobalPosition.DirectionTo(AIActor.GlobalPosition);
            Animation.Transition("Dodge");
            Movement.SetSpeed(Movement.DodgeSpeed);
            AIActor._CanRotate = false;
        }


        public override void PhysicsUpdate(double delta)
        {
            Movement.HandleMovement(dirToDodge, delta);
        }

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
            AIActor._CanRotate = true;
            target = null;
        }
    }
}