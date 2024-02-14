using Godot;
using Globals;
namespace Game
{
	[GlobalClass]
	public partial class Item : Node3D
	{
		[Export] public new string Name;
		[Export] public Texture Texture;
		[Export] public int Quantity;
		[Export] public string HoverText;

		[Export(PropertyHint.Enum, Slots.Passive+ ","+Slots.Head+","+ Slots.Body+","+ Slots.Legs+","+ Slots.RightHand+","+ Slots.LeftHand)]
		public string Type {get; set;} = Slots.Passive;
		public Actor Wielder = null;
	}
}
