using Godot;

namespace Game
{
    public interface IMeleeAttacker : IAttacker
    {
        void Attack1();

        void Attack2();

        void Attack3();

        void SprintLightAttack();

        void SprintHeavyAttack();

        void ComboAttack();

        void DodgeAttack();

        void JumpAttack();

        void HeavyAttack();
    }
}