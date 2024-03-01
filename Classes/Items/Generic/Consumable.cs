using Godot;
using Globals;
namespace Game
{
	[GlobalClass]
	public abstract partial class Consumable : Item
	{
		[Export(PropertyHint.Enum, ConsumableTypes.Heal)]
		public string ConsumableType {get; set;} = ConsumableTypes.Heal;

		[Export] private float value;

		public float Consume()
		{
			Quantity -= 1;
			return value;
		}
    }
}
