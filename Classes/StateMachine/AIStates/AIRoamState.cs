using Godot;
using System.Collections.Generic;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class AIRoamState : State
    {
        public override void Enter(Dictionary<string, Vector2> msg)
        {
            (Animation as AnimateAI).Transition("Roam");
            Movement.SetSpeed(Movement.WalkSpeed);
        }

        public override void Exit()
        {
        }

        public override void PhysicsUpdate(double delta)
        {
            (Actor as AI).Roam();
            GD.Print("Roaming");
        }

        public override void Update(double delta)
        {
        }
    }
}