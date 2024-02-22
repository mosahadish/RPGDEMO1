using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class Map : Node3D
	{	
        [Export] public Marker3D PlayerSpawnPos;

        public static void OnAIDeath(Actor actor)
        {
            GD.Print(actor.Name +" Died");
            if (actor is AI ai)
            {
                ai.SMachine.SetProcess(false);
                ai.SMachine.SetPhysicsProcess(false);
            }
            actor.QueueFree();
        }
    }
}