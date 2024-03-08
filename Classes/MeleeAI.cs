using System;
using Globals;
using Godot;

namespace Game
{
	[GlobalClass]
	public partial class MeleeAI : AI, IMeleeAttacker, IBlocker
	{
        private bool blocking;
        private bool blockedAttack = false;
        bool IBlocker.Blocking { get {return blocking;} set {blocking = value;} }
        bool IBlocker.AttackBlocked { get{return blockedAttack;} set {blockedAttack = value;}}

        public override void _Ready()
        {
            base._Ready();
            
            CurrentOffhand = (Weapon) GetNode("Visuals/Armature/Skeleton3D/BoneAttachment3D2/Node3D/Shield");
            CurrentOffhand.Wielder = this;

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
            blocking = true;
        }

        public void BlockRelease()
        {
            blockedAttack = false;
            blocking = false;
        }

        public void BlockHold()
        {
            (Animation as AnimateAI).CurrentMovementState = "Walk";
			(Animation as AnimateAI).BlockedAttack = false;

			blocking = true;
			blockedAttack = false;
        }

        public void BlockedAttack(float damage)
        {
            (Animation as AnimateAI).BlockedAttack = true;
            (Animation as AnimateAI).RequestOneShot("Offhand");
            Audio.Play(SoundEffects.ShieldBlock);
            blockedAttack = true;
        }

        public void BlockCounterAttack()
        {
            _CanRotate = false;
			_IsAttacking = true;
			
			(Animation as AnimateAI).MainAttack(Animations.CounterLightAttack);
			
			blockedAttack = false;
        }

        public bool IsBlocking()
        {
            return blocking;
        }

        public bool CanCounter()
        {
            return blockedAttack;
        }

        public void ResetBlockedAttack()
        {
            blockedAttack = false;
        }
        #endregion
    }
}
