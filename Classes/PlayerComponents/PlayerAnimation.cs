using Godot;
using System;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAnimation : Animate
    {

        public override void Transition(string transitionType, string transitionName)
        {
          AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
        }
    }
}
