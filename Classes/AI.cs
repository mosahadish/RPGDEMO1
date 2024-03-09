using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor, IDodger
	{
        [Export] public new AIStateMachine SMachine;
        [Export] public float LoseAggroRange;
        [Export] public float AttackRange;
        [Export] public float CircleRange;
        [Export] public float DodgeRange;
        [Export(PropertyHint.Range, "0,100,")] private float DodgeChancePercent;
        [Export(PropertyHint.Range, "0,100,")] private float AttackChancePercent;
        [Export(PropertyHint.Range, "0,100,")] private float BlockChancePercent;
        [Export] private float canDecideTimer;
		[Export] public Raycasts Raycasts;
		private Vector3 desiredVelo;
        
        public Player target;
        public Vector3 spawnPosition;
   
        private bool canDecide = true;
        private string action;
        private double timer = 0;
        private RandomNumberGenerator rng;
        private int rngResult;
        private float dodgeChance;
        private float attackChance;
        private float blockChance;
        private float healthWeight;

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

            dodgeChance = DodgeChancePercent;
            attackChance = AttackChancePercent;
            blockChance = BlockChancePercent;

            rng = new();


            if (SMachine != null)
            {
                ActorGotParried += SMachine.OnParry;
                ActorGotStaggered += SMachine.OnStagger;
            }
		}

        public override void _PhysicsProcess(double delta)
        {
            timer += delta;
            if (timer > rng.RandfRange(canDecideTimer - canDecideTimer*0.1f, canDecideTimer + canDecideTimer/2)) 
            {   
                canDecide = true;
                timer = 0;
            }
        }

        public override void GotParried()
        {
            base.GotParried();
            timer = 0;
            canDecide = false;
        }

        public string DecideOnNextAction(float distToTarget)
        {
            //Reset to default
            dodgeChance = DodgeChancePercent;
            attackChance = AttackChancePercent;
            blockChance = BlockChancePercent;

            healthWeight = (100-HP.AsPercent())/6;
            
            dodgeChance += healthWeight;
            blockChance += healthWeight;


            if (_IsAttacking) return null;

            if (SMachine.target.HasAimingWeapon())
            {
                if (ShouldBlock()) 
                {
                    rngResult = rng.RandiRange(0,99);
                    return nameof(AIBlockState);
                }
            }

            if (distToTarget <= AttackRange)
            {
                if (ShouldBlock())
                {
                    rngResult = rng.RandiRange(0,99);
                    return nameof(AIBlockState);
                }
            }

            if (distToTarget <= DodgeRange)
            {
                if (SMachine.target.IsAttacking() && ShouldDodge())
                {
                    rngResult = rng.RandiRange(0,99);
                    return nameof(AIDodgeState);
                }
            }

            if (canDecide == false) return null;

            canDecide = false;
            rngResult = rng.RandiRange(0,99);


            if (distToTarget <= AttackRange) 
            {
                if (ShouldAttack())
                {
                    timer = 0;
                    return nameof(AIAttackState);
                }
            }

            return action;
        }

        public bool ShouldBlock()
        {
            return rngResult < blockChance;
        }
        
        public bool ShouldDodge(bool roll = false)
        {
            if (roll) rngResult = rng.RandiRange(0,99);
            
            return rngResult < dodgeChance;
        }

        private bool ShouldAttack()
        {
            return rngResult <= attackChance;
        }

    
        public void Dodge()
        {
            (Animation as AnimateAI).RequestOneShot("Dodge");
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

        #region Testing Steering Behavior
        //https://code.tutsplus.com/understanding-steering-behaviors-wander--gamedev-1624t
        Vector3 circleCenter;
        Vector3 displacement;
        Vector3 wanderForce;
        float circleDistance = 0.5f;
        float wanderAngle = 45;
        float ANGLE_CHANGE = 25;

        public Vector3 DisplacementTest()
        {
            circleCenter = Velocity.Normalized();
            circleCenter *= circleDistance;

            displacement = new Vector3(0,0,-1f);
            displacement *= circleDistance;
            displacement = displacement.Rotated(Vector3.Up, Mathf.DegToRad(wanderAngle));

            wanderAngle += (rng.Randf() *ANGLE_CHANGE) - (ANGLE_CHANGE *0.5f);

            wanderForce = circleCenter + displacement;
            return wanderForce;
        }
        #endregion

    }
}