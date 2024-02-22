using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class MeleeAI : AI, IMeleeAttacker
	{
        
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
