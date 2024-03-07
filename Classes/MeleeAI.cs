using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class MeleeAI : AI, IMeleeAttacker, IBlocker
	{
        private bool blocking;
        bool IBlocker.Blocking { get {return blocking;} set {blocking = value;} }
        bool IBlocker.AttackBlocked { get; set;}

        public override void _Ready()
        {
            base._Ready();
            (Animation as AnimateAI).CurrentWeaponState = "Sword";
            (Animation as AnimateAI).CurrentOffhandState = "Shield";
            (Animation as AnimateAI).CurrentMovementState = "Roam";
        }

        #region Attack
        public void Attack1()
        {
            (Animation as AnimateAI).MainAttack("Attack1");;
            // (Animation as AnimateAI).Transition("Attack");
            // (Animation as AnimateAI).NodeTransition("Attack1");
			_IsAttacking = true;
            Audio.Play(SoundEffects.SwordAttack1);
        }

        public void Attack2()
        {
            (Animation as AnimateAI).MainAttack("Attack2");
			_IsAttacking = true;
            Audio.Play(SoundEffects.SwordAttack2);
        }

        public void Attack3()
        {
            (Animation as AnimateAI).MainAttack("Attack3");
			_IsAttacking = true;
            Audio.Play(SoundEffects.SwordAttack3);
        }

        public void ComboAttack()
        {
            (Animation as AnimateAI).MainAttack("ComboAttack");
			_IsAttacking = true;
        }

        public void FinishAttacking()
        {
            /*
            Usually called when exiting AttackState
            */
            _IsAttacking = false;
            WeaponDamageOff(); //anti bug measure when animation stops midway (stagger etc)
			Movement.SetSpeed(Movement.CurrentSpeed);
        }

        public bool IsAttacking()
        {
            return _IsAttacking;
        }

        public void DodgeLightAttack()
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

        
        #endregion

        #region Block
        public void Block()
        {
            
        }

        public void BlockRelease()
        {
            
        }

        public void BlockHold()
        {
            
        }

        public void BlockedAttack(float damage)
        {
            
        }

        public void BlockCounterAttack()
        {
            
        }

        public bool IsBlocking()
        {
            return blocking;
        }

        public bool CanCounter()
        {
            return true;
        }
        #endregion
    }
}
