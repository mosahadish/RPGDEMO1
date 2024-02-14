using Godot;
using System;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class AI : Actor
	{
        public void Roam()
        {
            Velocity = Velocity + Vector3.Down;

            MoveAndSlide();
        }
    }
}