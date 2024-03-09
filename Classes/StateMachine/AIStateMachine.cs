using GameSettings;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [GlobalClass]
    public partial class AIStateMachine : Node
    {
        Signal transitionedTo;
        
        [Export] AIActionStateMachine aMachine;
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
        private bool chaseOutOfRange = true; //This is for when other enemies alert this one, or detected projectile

        string nextAction = "";
    
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
                if (state is AIParriedState) return;
                if (state is AIStaggerState) return;
                if (state is AIDodgeState) return;
                if (state is AIAttackState) return;


                if (target != null) distToTarget = AIActor.GlobalPosition.DistanceTo(target.GlobalPosition);
                else distToTarget = 9999;
                
                
                if (chaseOutOfRange == false)
                    if (distToTarget > AIActor.LoseAggroRange && target != null) target = null;

                if(target == null && state is AIRoamState == false) 
                {
                    TransitionTo(nameof(AIRoamState));
                    chaseOutOfRange = true;
                }

                // if (target != null && distToTarget <= AIActor.DodgeRange)
                // {
                //     if (AIActor.ShouldDodge()) TransitionTo(nameof(AIDodgeState));
                // }

                if (target != null && distToTarget <= AIActor.CircleRange) 
                {
                    chaseOutOfRange = false;
                    if (state is AIEngageState == false && state is AIAttackState == false)
                        TransitionTo(nameof(AIEngageState));
                    else 
                    {
                        nextAction = AIActor.DecideOnNextAction(distToTarget);
                        if (nextAction == nameof(AIBlockState))
                        {
                            if (aMachine.state is AIBlockState == false)
                                aMachine.TransitionTo(nextAction);
                        }
                        else
                        {
                            if (aMachine.state is AIIdleState)
                                TransitionTo(nextAction);
                        }
                    }
                }
                else if (target != null && distToTarget > AIActor.CircleRange +1) 
                {
                    if (state is AIChaseState == false && state is AIAttackState == false) TransitionTo(nameof(AIChaseState));
                }
            }
        }

        public void TransitionTo(string TargetStateName)
        {
            lock(this)
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
            GD.Print(anim);
            if (anim.Contains("Parried") == false)
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

        private void OnArea3DAreaEntered(Node3D area)
        {
            if (area.GetParent() is Projectile proj) 
            {
                if (proj.Spawner is Player player) target = player;
                if (state is AIAttackState || state is AIStaggerState) return;
                
                if (AIActor.ShouldDodge(true)) TransitionTo(nameof(AIDodgeState));
            }
        }

        public void OnStagger()
        {
            aMachine.TransitionTo(nameof(AIIdleState));
            TransitionTo(nameof(AIStaggerState));
        }

        public void OnParry()
        {
            aMachine.TransitionTo(nameof(AIIdleState));
            TransitionTo(nameof(AIParriedState));
        }
    }   
}