using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor, IDodger
	{
        [Export] public new AIStateMachine SMachine;
        [Export] public float AttackRange;
        [Export] public float CircleRange;
        [Export] public float DodgeRange;
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
            
            if (distToTarget <= DodgeRange)
            {
                rngResult = rng.RandiRange(0,9);
                if (SMachine.target.IsAttacking() && rngResult <= 3)
                {
                    return nameof(AIDodgeState);
                }
            }
            if (distToTarget <= AttackRange) 
            {
                action =  nameof(AIAttackState);
            }
           
            canDecide = false;
            return action;
        }

        public void Dodge()
        {
            (Animation as AnimateAI).Transition("Dodge");
            Movement.SetSpeed(Movement.DodgeSpeed);
            _CanRotate = false;
            dodging = true;
        }

        public void FinishDodging()
        {
            _CanRotate = true;
            dodging = false; 
        }

        public bool IsDodging()
        {
            return dodging;
        }
    }
}