using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class Map : Node3D
	{	

        [Export] public Marker3D PlayerSpawnPos;

        public void OnAIDeath(Actor actor)
        {
            GD.Print(actor +" Died");
            actor.QueueFree();
        }
    }
}