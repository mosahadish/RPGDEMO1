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

        public virtual void Transition(string transitionType, string transitionName) {}

        public void BlendPosition(string blendType, Vector2 blendValue)
        {
            AnimTree.Set("parameters/" + blendType + "/blend_position", blendValue);
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