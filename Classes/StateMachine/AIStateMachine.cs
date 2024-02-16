using Globals;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

namespace Game
{
    [GlobalClass]
    public partial class AIStateMachine : StateMachine
    {

        private List<AI> bodiesToNotifyOfTarget = new();
        private float distToTargetSqr; //Distance squared is faster to calculate
        private Actor target;

        bool chaseFlag = true;

        public override void _PhysicsProcess(double delta)
        {
            //GD.Print(state.Name);
            //GD.Print(distToTarget);
            base._PhysicsProcess(delta);
            if (target != null) distToTargetSqr = Actor.GlobalPosition.DistanceSquaredTo(target.GlobalPosition);
            else distToTargetSqr = 9999;

            if (distToTargetSqr > 125 && target != null) TargetGone();
            if (target != null && distToTargetSqr > 4 && distToTargetSqr <= 16) TargetInCircleRange();
            if (target != null && distToTargetSqr <= 4) TargetInAttackRange();
            if (target != null && distToTargetSqr > 16) TargetInChaseRange();
        }

        private void TargetInCircleRange()
        {
            if (state is AIAttackState) return;

            if (state is AICircleState == false)
            {
                TransitionTo("AICircleState", Msg);
                (state as AICircleState).SetTarget(target);
            }
            Msg.Clear();
        }

        public void TargetInChaseRange()
        {
            if (state is AIChaseState) return;
            TransitionTo("AIChaseState", null);
            (state as AIChaseState).SetTarget(target);
            Msg.Clear();
        }

        public void TargetGone()
        {
            target = null;
            if (state is AIChaseState)
                (state as AIChaseState).SetTarget(null);
            TransitionTo("AIRoamState", null);
            distToTargetSqr = 9999;
            Msg.Clear();
        }

        public void TargetInAttackRange()
        {
            Msg[Animations.Attack1] = Vector2.Zero;
            if (state is AIAttackState == false)
            {
                TransitionTo("AIAttackState", Msg);
                (state as AIAttackState).SetTarget(target);
            }
            Msg.Clear();
        }

        private void OnAnimationFinished(string anim)
        {
            Msg.Clear();
            GD.Print(anim);
            if (target == null) 
            {
                TransitionTo("AIRoamState", Msg);
                return;
            }
            else if (anim.Contains(Animations.Attack1) && distToTargetSqr < 4)
            {
                Msg[Animations.Attack2] = Vector2.Zero;
                TransitionTo("AIAttackState", Msg);
                (state as AIAttackState).SetTarget(target);
                return;
            }
            else if (anim.Contains(Animations.Attack2) && distToTargetSqr < 4)
            {
                Msg[Animations.Attack3] = Vector2.Zero;
                TransitionTo("AIAttackState", Msg);
                (state as AIAttackState).SetTarget(target);
                return;
            }
            else if (anim.Contains(Animations.Attack3))
            {
               TargetInChaseRange();
               return;
            }
            
            TransitionTo("AIChaseState", Msg);
            (state as AIChaseState).SetTarget(target);
        }


        public override void HandleAttackInput(Dictionary<string, bool> Msg)
        {
        }

        public override void HandleMovementInput(Dictionary<string, Vector2> Msg)
        {
        }


         private void OnArea3DEntered(Node3D body)
		{
			if (body is Player player)
			{
				target = player;
				GD.Print("Player detected");
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

    }
}