using System.Collections.Generic;
using System.Text;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Animate : Node
    {
        [Export] public AnimationTree AnimTree;
        [Export] public AnimationPlayer AnimPlayer;

        private Vector2 currentBlendPos;

        public virtual void Transition(string transitionType, string transitionName) {}

        public void BlendPosition(string blendType, Vector2 blendValue)
        {
            currentBlendPos = (Vector2)AnimTree.Get("parameters/" + blendType + "/blend_position");

            AnimTree.Set("parameters/" + blendType + "/blend_position", currentBlendPos.Slerp(blendValue, 0.05f));
        }    

        public void OneShot(string name)
        {
            AbortOneShot(name);
            AnimTree.Set("parameters/"+name+"/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        public void AbortOneShot(string name)
        {
            AnimTree.Set("parameters/"+name+"/request", (int)AnimationNodeOneShot.OneShotRequest.Abort);
        }
    }
}