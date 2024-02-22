using GameSettings;
using Globals;
using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Game
{
    [GlobalClass]
    public partial class AIStateMachine : Node
    {
        Signal transitionedTo;

        [Export] Label3D StateLabel;
        [Export] Label3D DistLabel;
        [Export] public AIState initialState;

        [Export] public AI AIActor;
        public AIState state;
        
        public Movement Movement;
        public Attack Attack;

        public Player target;

        private List<AI> bodiesToNotifyOfTarget = new();
        private float distToTarget = 9999; //Distance squared is faster to calculate, if necessary do it
    
        public async override void _Ready()
        {
            await ToSignal(GetParent(), "ready");
            state = initialState;
            
            Movement = AIActor.Movement;
            Attack = AIActor.Attack;

            foreach (AIState c in GetChildren().Cast<AIState>())
            {
                c.AIActor = AIActor;
                c.Movement = Movement;
                c.Animation = (AnimateAI)AIActor.Animation;
                c.Attack = Attack;
                c.Stam = AIActor.Stam;                   
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
            
            if (Settings.Debug == true)
            {
                StateLabel.Show();
                DistLabel.Show();
            }
            else
            {
                StateLabel.Hide();
                DistLabel.Hide();
            }
            
            StateLabel.Text = state.Name;
            DistLabel.Text = distToTarget.ToString();

            if (AIActor.IsOnFloor() == false)
			{
				if ((state is AIAirState) == false) TransitionTo(nameof(AIAirState));
			}
            else
            {
                if (state is AIStaggerState) return;
                if (state is AIDodgeState) return;

                if (target != null) distToTarget = AIActor.GlobalPosition.DistanceTo(target.GlobalPosition);
                else distToTarget = 9999;
                
                

                if (distToTarget > 15 && target != null) target = null;

                if(target == null && state is AIRoamState == false) TransitionTo(nameof(AIRoamState));


                if (target != null && distToTarget <= AIActor.CircleRange) 
                {
                    if (state is AIEngageState == false && state is AIAttackState == false)
                        TransitionTo(nameof(AIEngageState));
                    else 
                    {
                        TransitionTo(AIActor.DecideOnNextAction(distToTarget));
                    }
                }
                //if (target != null && distToTarget <= 4) TransitionTo("AIAttackState");
                else if (target != null && distToTarget > AIActor.CircleRange +1) 
                {
                    if (state is AIChaseState == false && state is AIAttackState == false) TransitionTo(nameof(AIChaseState));
                }
            }
        }

        public void TransitionTo(string TargetStateName)
        {
            if (HasNode(TargetStateName))
            {
                state.Exit();
                state = GetNode(TargetStateName) as AIState;
                state.Enter(target);
                //EmitSignal()
            }
        }


        private void OnAnimationFinished(string anim)
        {
            TransitionTo(nameof(AIEngageState));
        }

         private void OnArea3DEntered(Node3D body)
		{
			if (body is Player player)
			{
				target = player;
			}
			if (body is AI aI)
			{
				bodiesToNotifyOfTarget.Add(aI);
			}
		}

        private void OnArea3DExited(Node3D body)
		{
			if (body is Player)
				{
					// if (body == Target)
					// {
					// 	Target = null;
					// 	(SMachine as AIStateMachine).TargetGone();
					// 	distToTarget = 9999;
					// }
				}
				
			if (body is AI aI)
			{
				if (bodiesToNotifyOfTarget.Contains(aI)) bodiesToNotifyOfTarget.Remove(aI);
			}
		}

        public void OnStagger()
        {
            TransitionTo(nameof(AIStaggerState));
        }
    }   
}