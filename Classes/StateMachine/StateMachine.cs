using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [GlobalClass]
    public abstract partial class StateMachine : Node
    {

        Signal transitionedTo;

        [Export] public State initialState;

        [Export] public Actor Actor;
        public State state;

        public Movement Movement;
        public Attack Attack;
    

        public async override void _Ready()
        {
            await ToSignal(GetParent(), "ready");
            Dictionary<string, Vector2> Msg = new();
            state = initialState;
            
            Movement = Actor.Movement;
            Attack = Actor.Attack;

            foreach (State c in GetChildren().Cast<State>())
            {
                c.Actor = Actor;
                c.Movement = Movement;
                c.Animation = Actor.Animation;
                c.Attack = Attack;
                c.Stam = Actor.Stam;

                if (Actor is Player && (c is PlayerRunState))
                    (c as PlayerRunState).SetCamera((Actor as Player).Camera);
                if (Actor is Player && (c is PlayerDodgeState))
                    (c as PlayerDodgeState).SetCamera((Actor as Player).Camera);
                if (Actor is Player && (c is PlayerAttackState))
                    (c as PlayerAttackState).SetCamera((Actor as Player).Camera);
                    
            }

            state.Enter(Msg);
        }
        
        public override void _Process(double delta)
        {
            state.Update(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            state.PhysicsUpdate(delta);
        }

        public void TransitionTo(string TargetStateName, Dictionary<string, Vector2> Msg)
        {
            if (HasNode(TargetStateName))
            {
                state.Exit();
                state = GetNode(TargetStateName) as State;
                state.Enter(Msg);
                //EmitSignal()
            }
        }

        public abstract void HandleAttackInput(Dictionary<string, bool> Msg);

        public abstract void HandleMovementInput(Dictionary<string, Vector2> Msg);
    }
}