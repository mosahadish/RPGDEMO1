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

        //Testing, making it like the player's
        public string CurrentMovementState = "Run";
        public string CurrentWeaponState = "Unarmed";
        public string CurrentOffhandState = "Unarmed";
        public bool Blocking = false;
        public bool BlockedAttack = false;
        public string CurrentAttack = "";
 

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
        public override void Transition(string transitionType, string transitionName)
        {
            AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
        }


        public void MainAttack(string attack)
        {
            CurrentAttack = attack;
            GD.Print(CurrentAttack);
            RequestOneShot("MainAttack");
        }

        public void RangeAttack(string attack)
        {
            CurrentAttack = attack;
            RequestOneShot("RangeAttack");
        }

        public void OffhandBlend(double val)
        {
         AnimTree.Set("parameters/OffhandBlend/blend_amount", val);
        }

        public void BlendMovement(Vector2 blendPos)
        {
            AnimTree.Set("parameters/MovementState/" + CurrentWeaponState + "/" + CurrentMovementState + "/blend_position", blendPos);
        }

        public void RequestOneShot(string oneShotName)
        {
            CurrentOneShot = oneShotName;
            AnimTree.Set("parameters/" + oneShotName + "/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }
    }
}