using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIIdleState : AIState
    {
        public override void Enter(Player target)
        {   
            this.target = target;
        }


        public override void PhysicsUpdate(double delta)
        {

        }   

        public override void Update(double delta)
        {
           
        }

        public override void Exit()
        {
           target = null;
        }
    }
}