using System.Collections.Generic;
using System.Text;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AnimateAI : Animate
    {

       public void Transition(string stateName)
       {
            AnimTree.Set("parameters/conditions/" + stateName, true);
       }
    }
}