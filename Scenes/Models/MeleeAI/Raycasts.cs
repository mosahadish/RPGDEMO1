using Godot;
using System;

namespace Game
{
    [GlobalClass]
    public partial class Raycasts : Node3D
    {
        [Export] private RayCast3D rayFloor;

        private bool floorDetected = true;

        public override void _PhysicsProcess(double delta)
        {
            if (rayFloor.IsColliding())
            {
                floorDetected = true;
            }

            else floorDetected = false;
        }

        public bool IsOnFloor()
        {
            return floorDetected;
        }
    }
}
