using System.Collections.Generic;
using System.Text;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AnimateAI : Animate
    {
    
        public string CurrentState = "";
        public string CurrentNodeInState = "";

        public void Transition(string stateName)
        {
           // GD.Print("Switching from State: " + CurrentState +" to State: " + stateName);
            AnimTree.Set("parameters/" + CurrentState+ "/conditions/" + CurrentNodeInState, false);
            
            AnimTree.Set("parameters/conditions/" + CurrentState, false);
            AnimTree.Set("parameters/conditions/" + stateName, true);
            CurrentState = stateName;
        }

        public void NodeTransition(string nodeName)
        {
            //GD.Print("Current state is " + CurrentState);
            if (CurrentNodeInState != "")
            {
                //GD.Print("Switching from " + CurrentNodeInState + " to " + nodeName);
                AnimTree.Set("parameters/" + CurrentState+ "/conditions/" + CurrentNodeInState, false);
            }

            AnimTree.Set("parameters/" + CurrentState+ "/conditions/" + nodeName, true);
            CurrentNodeInState = nodeName;
        }

        public override void BlendPosition(string blendType, Vector2 blendValue)
        {
            AnimTree.Set("parameters/"+CurrentState+"/"+CurrentNodeInState+"/blend_position", blendValue);
        }
    }
}