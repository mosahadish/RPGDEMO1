using Game;
using Godot;
using System;

namespace Game
{
    [GlobalClass]
    public partial class Shield : Weapon
    {
    
        public override void _Ready()
        {
            base._Ready();
            _CanBlock = true;
        }
    }
}
