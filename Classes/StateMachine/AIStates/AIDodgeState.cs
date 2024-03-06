using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AIDodgeState : AIState
    {
        private Vector3 dirToDodge;

        public override void Enter(Player target)
        {   
            //dirToDodge = target.GlobalPosition.DirectionTo(AIActor.GlobalPosition);
            dirToDodge = AIActor.Velocity.Normalized();
            AIActor.Dodge();
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
            AIActor.FinishDodging();
            target = null;
        }
    }
}