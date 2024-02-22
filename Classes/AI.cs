using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor, IMeleeAttacker, IDodger
	{
        [Export] public new AIStateMachine SMachine;
        [Export] public float AttackRange;
        [Export] public float CircleRange;
        //[Export] public new AIStateMachine SMachine;
		[Export] public Raycasts Raycasts;
		private Vector3 desiredVelo;
        
        
        public Player target;
   
        private bool canDecide = true;
        private string action;
        private double timer = 0;
        private RandomNumberGenerator rng;
        private int rngResult;

        private bool dodging = false;
        bool IDodger.Dodging
    	{
        get { return dodging; }
        set { dodging = value; }
    	}


        public override void _Ready()
        {
            base._Ready();
            if (CurrentWeapon != null)
			    CurrentWeapon.Wielder = this;
            if (CurrentOffhand != null)
                CurrentOffhand.Wielder = this;

            rng = new();

			if (GetParent() is Map map)
			    ActorDeathWithArgument += Map.OnAIDeath;
		}

        public override void _PhysicsProcess(double delta)
        {
            timer += delta;

            if (timer > rng.RandfRange(2, 2.5f)) 
            {
                canDecide = true;
                timer = 0;
            }
        }

        public void ApplySteeringForce(Vector3 globalTargetPos, double delta)
		{
			desiredVelo = globalTargetPos - GlobalPosition;
			Velocity = desiredVelo + Velocity;
		}

        public string DecideOnNextAction(float distToTarget)
        {
            if (canDecide == false) return null;

            action = nameof(AIEngageState);

            if (distToTarget <= AttackRange) 
            {
                rngResult = rng.RandiRange(0,9);
                if (SMachine.target.IsAttacking() && rngResult <= 3)
                {
                    action = nameof(AIDodgeState);
                }
                else
                {
                    action =  nameof(AIAttackState);
                }
            }
           
            canDecide = false;
            return action;
        }

        public void Attack1()
        {
            (Animation as AnimateAI).Transition(Animations.Attack1);
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack1);
        }

        public void Attack2()
        {
            (Animation as AnimateAI).Transition(Animations.Attack2);
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack2);
        }

        public void Attack3()
        {
            (Animation as AnimateAI).Transition(Animations.Attack3);
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack3);
        }

        public void ComboAttack()
        {
            (Animation as AnimateAI).Transition(Animations.ComboAttack);
			_IsAttacking = true;
        }

        public void FinishAttacking()
        {
            _IsAttacking = false;
            WeaponDamageOff(); //anti bug measure when animation stops midway (stagger etc)
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

        public void Dodge()
        {
            (Animation as AnimateAI).Transition("Dodge");
            Movement.SetSpeed(Movement.DodgeSpeed);
            _CanRotate = false;
        }

        public void FinishDodging()
        {
            _CanRotate = true;
        }

        public bool IsDodging()
        {
            return dodging;
        }
    }
}