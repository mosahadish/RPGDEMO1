using Godot;

namespace Game
{
    public interface IMeleeAttacker
    {
        void Attack1();

        void Attack2();

        void Attack3();

        bool IsAttacking();

        void FinishAttacking();
    }
}