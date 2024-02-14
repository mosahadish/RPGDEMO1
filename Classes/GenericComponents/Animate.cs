using System.Collections.Generic;
using System.Text;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Animate : Node
    {
       [Export] public AnimationTree AnimTree;
       [Export] public AnimationPlayer AnimPlayer;

       public void Transition(string transitionType, string transitionName)
       {
            AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
       }

       public void BlendPosition(string blendType, Vector2 blendValue)
       {
            AnimTree.Set("parameters/" + blendType + "/blend_position", blendValue);
       }    

        public void OneShot(string name)
        {
            AbortOneShot();
            AnimTree.Set("parameters/"+name+"/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        public void AbortOneShot()
        {
            AnimTree.Set("parameters/OneAttack/request", (int)AnimationNodeOneShot.OneShotRequest.Abort);
        }
    }
}