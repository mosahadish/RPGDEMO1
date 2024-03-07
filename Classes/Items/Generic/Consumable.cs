using Godot;
using Globals;
namespace Game
{
	[GlobalClass]
	public abstract partial class Consumable : Item
	{
		[Export(PropertyHint.Enum, ConsumableTypes.Heal)]
		public string ConsumableType {get; set;}

		[Export] private float value;
		[Export] public int MaxQuantity;

		public float Consume()
		{
			Quantity -= 1;
			return value;
		}
    }
}
