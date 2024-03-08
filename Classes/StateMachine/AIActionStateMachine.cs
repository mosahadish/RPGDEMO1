using System.Linq;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AIActionStateMachine : Node
    {
        [Export] AIState initialState;
        public AIState state;
        [Export] public AI AI;
        public AnimateAI Animation;
        public Movement Movement;
        public Stamina Stam;


        public async override void _Ready()
        {
            await ToSignal(GetParent(), "ready");
            state = initialState;
            
            Movement = AI.Movement;
            Animation = AI.Animation as AnimateAI;
            Stam = AI.Stam;

            foreach (AIState c in GetChildren().Cast<AIState>())
            {
                c.AIActor = AI;
                c.Movement = Movement;
                c.Animation = Animation;
                c.Stam = Stam;
                

                if (c.HasSignal("StateFinishedWithArgument"))
                {
                    c.StateFinishedWithArgument += StateFinished;
                }
            }

            state.Enter(null);
        }
        
        public override void _Process(double delta)
        {
            state.Update(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            state.PhysicsUpdate(delta);
        }

        public void TransitionTo(string TargetStateName)
        {
            lock(this)
            if (HasNode(TargetStateName))
            {
                state.Exit();
                state = GetNode(TargetStateName) as AIState;
                state.Enter(null);
                //EmitSignal()
            }
        }

        private void StateFinished(string stateName)
        {
            if (stateName == nameof(AIBlockState))
            {
                TransitionTo(nameof(AIIdleState));
            }
        }
    }
}