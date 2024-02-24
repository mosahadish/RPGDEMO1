using Godot;
using Globals;
namespace Game
{
	[GlobalClass]
	public partial class Consumable : Item
	{
		[Export(PropertyHint.Enum, ConsumableTypes.Heal)]
		public string ConsumableType {get; set;} = ConsumableTypes.Heal;

		
    }
}
