using Godot;
namespace Game
{
    public interface IBlocker
    {
        void Block();

        void BlockRelease();

        void BlockHold();

        void BlockedAttack();

        void BlockCounterAttack();

        bool IsBlocking();
    }
}