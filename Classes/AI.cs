using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor, IMeleeAttacker
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
        

        public override void _Ready()
        {
            base._Ready();
			CurrentWeapon.Wielder = this;
 

			if (GetParent() is Map map)
			    ActorDeathWithArgument += map.OnAIDeath;
		}

        public override void _PhysicsProcess(double delta)
        {
            timer += delta;

            if (timer > 2.5) 
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

            if (distToTarget <= AttackRange) action =  "AIAttackState";
            //else if (HP.GetValue() / HP.MaxValue < 0.3) action = "Retreat";
            else action = "AIEngageState";
            
            canDecide = false;
            return action;
        }

        // public bool ShouldAttack()
        // {
            
        // }

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
    }
}