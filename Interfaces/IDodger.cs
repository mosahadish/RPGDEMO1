using Godot;
namespace Game
{
    public interface IDodger
    {
        protected bool Dodging { get; set; }

        void Dodge();

        void FinishDodging();

        bool IsDodging();
    }
}