using Godot;
namespace Game
{
    public interface IBlocker
    {
        protected bool Blocking { get; set; }
        protected  bool AttackBlocked { get; set; }

        void Block();

        void BlockRelease();

        void BlockHold();

        void BlockedAttack(float damage);

        void BlockCounterAttack();

        bool IsBlocking();

        bool CanCounter();

        void ResetBlockedAttack();
    }
}