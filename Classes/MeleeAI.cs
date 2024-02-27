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
            (Animation as AnimateAI).Transition("Attack");
            (Animation as AnimateAI).NodeTransition("Attack1");
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack1);
            // Animation.AnimTree.Set("parameters/Attack/conditions/Attack1", false);
        }

        public void Attack2()
        {
            (Animation as AnimateAI).Transition("Attack");
            (Animation as AnimateAI).NodeTransition("Attack2");
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack2);
            // Animation.AnimTree.Set("parameters/Attack/conditions/Attack2", false);
        }

        public void Attack3()
        {
            (Animation as AnimateAI).Transition("Attack");
            (Animation as AnimateAI).NodeTransition("Attack3");
			_IsAttacking = true;
            audio.Play(SoundEffects.SwordAttack3);
            // Animation.AnimTree.Set("parameters/Attack/conditions/Attack3", false);
        }

        public void ComboAttack()
        {
            (Animation as AnimateAI).Transition("Attack");
            (Animation as AnimateAI).NodeTransition("ComboAttack");
            //Animation.AnimTree.Set("parameters/Attack/conditions/ComboAttack", true);
			_IsAttacking = true;

            // Animation.AnimTree.Set("parameters/Attack/conditions/ComboAttack", false);
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

    }
}
