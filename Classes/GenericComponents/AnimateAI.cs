using System.Collections.Generic;
using System.Text;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AnimateAI : Animate
    {
    
        public string CurrentState = "Roam";

        public void Transition(string stateName)
        {
            AnimTree.Set("parameters/conditions/" + CurrentState, false);
            AnimTree.Set("parameters/conditions/" + stateName, true);
            CurrentState = stateName;
        }
    }
}