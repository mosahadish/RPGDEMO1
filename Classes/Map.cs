using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class Map : Node3D
	{	
        public void OnAIDeath(Actor actor)
        {
            GD.Print(actor +" Died");
            actor.QueueFree();
        }
    }
}