using System.Linq;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class ActionStateMachine : Node
    {
        [Export] State initialState;
        public State state;
        [Export] public Player Player;
        public PlayerAnimation Animation;
        public PlayerMovement Movement;
        public Stamina Stam;


        public async override void _Ready()
        {
            await ToSignal(GetParent(), "ready");
            state = initialState;
            
            Movement = Player.Movement;
            Animation = Player.Animation as PlayerAnimation;
            Stam = Player.Stam;

            foreach (State c in GetChildren().Cast<State>())
            {
                c.Actor = Player;
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
                state = GetNode(TargetStateName) as State;
                state.Enter(null);
                //EmitSignal()
            }
        }

        private void StateFinished(string stateName)
        {
            if (stateName == nameof(PlayerDrinkState))
            {
                TransitionTo(nameof(PlayerIdleState));
            }
        }
    }
}