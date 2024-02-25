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
        [Export] private float DodgeChance;
        [Export] private float canDecideTimer;
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

            DodgeChance *= 10;

			if (GetParent() is Map map)
			    ActorDeathWithArgument += Map.OnAIDeath;
		}

        public override void _PhysicsProcess(double delta)
        {
            timer += delta;
            //Velocity += DisplacementTest();
            Velocity = myFuncs.LerpVector3(Velocity, Velocity + DisplacementTest(), 0.1f);

            if (timer > rng.RandfRange(canDecideTimer - 0.7f, canDecideTimer + 1.2f)) 
            {
                canDecide = true;
                timer = 0;
            }
        }

        public string DecideOnNextAction(float distToTarget)
        {
            if (canDecide == false) return null;

            action = nameof(AIEngageState);
            
            if (distToTarget <= DodgeRange)
            {
                DodgeChance += (1-HP.AsPercent())/6;

                rngResult = rng.RandiRange(0,9);
                if (ShouldDodge())
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

        private bool ShouldDodge()
        {
            return SMachine.target.IsAttacking() && rngResult <= DodgeChance;
        }

        
        //https://code.tutsplus.com/understanding-steering-behaviors-wander--gamedev-1624t
        Vector3 circleCenter;
        Vector3 displacement;
        Vector3 wanderForce;
        float circleDistance = 0.1f;
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