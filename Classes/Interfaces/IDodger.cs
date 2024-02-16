using Godot;
namespace Game
{
    public interface IDodger
    {
        void Dodge();

        void FinishDodging();

        bool IsDodging();
    }
}