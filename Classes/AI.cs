using Globals;
using Godot;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor, IMeleeAttacker
	{
		[Export] public Raycasts Raycasts;
		
		private Vector3 desiredVelo;
		private List<AI> bodiesToNotifyOfTarget = new();

        public override void _Ready()
        {
            base._Ready();
			CurrentWeapon.Wielder = this;

			if (GetParent() is Map map)
			{
				ActorDeathWithArgument += map.OnAIDeath;
			}
		}

        public void ApplySteeringForce(Vector3 globalTargetPos, double delta)
		{
			desiredVelo = globalTargetPos - GlobalPosition;
			Velocity = desiredVelo + Velocity;
		}

        public void Attack1()
        {
            (Animation as AnimateAI).Transition(Animations.Attack1);
			_IsAttacking = true;
        }

        public void Attack2()
        {
            (Animation as AnimateAI).Transition(Animations.Attack2);
			_IsAttacking = true;
        }

        public void Attack3()
        {
            (Animation as AnimateAI).Transition(Animations.Attack3);
			_IsAttacking = true;
        }

        public void FinishAttacking()
        {
            _IsAttacking = false;
			Movement.SetSpeed(Movement.CurrentSpeed);
        }

        public bool IsAttacking()
        {
            return _IsAttacking;
        }

        public void DodgeAttack()
        {
            
        }

        public void JumpAttack()
        {
            
        }

        public void HeavyAttack()
        {
            
        }

        public void SprintLightAttack()
        {
            
        }

        public void SprintHeavyAttack()
        {
        }
    }
}