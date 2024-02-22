using Godot;

namespace Game
{
    public interface IAttacker
    {
        bool IsAttacking();

        void FinishAttacking();
    }
}
