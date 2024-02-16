using System.Collections.Generic;
using System.Text;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AnimateAI : Animate
    {
    
        private string currentState = "Roam";

        public void Transition(string stateName)
        {
            AnimTree.Set("parameters/conditions/" + currentState, false);
            AnimTree.Set("parameters/conditions/" + stateName, true);
            currentState = stateName;
        }
    }
}