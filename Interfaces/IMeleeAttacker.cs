using Godot;

namespace Game
{
    public interface IMeleeAttacker
    {
        void Attack1();

        void Attack2();

        void Attack3();

        void SprintLightAttack();

        void SprintHeavyAttack();

        void DodgeAttack();

        void JumpAttack();

        void HeavyAttack();

        bool IsAttacking();

        void FinishAttacking();
    }
}