using Godot;
using System;
using Globals;

namespace Game
{
    [GlobalClass]
    public partial class PlayerAnimation : Animate
    {

        // public override void Transition(string transitionType, string transitionName)
        // {
        //   AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
        // }

        public string CurrentMovementState = "Run";
        public string CurrentWeaponState = "Unarmed";
        public string CurrentAttunement = "Fire";
        public string PreviousAttack = "";

        public override void _Ready()
        {
            base._Ready();
            WeaponTransition("Unarmed");
            UpdateAttunement(CurrentAttunement);
        }

        public override void Transition(string transitionType, string transitionName)
        {
          AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
        }

        public void MovementTransition(string movementType)
        { //set to walk, run, sprint etc
          AnimTree.Set("parameters/MovementState/" + CurrentWeaponState + "/conditions/" + movementType, true);
          
          CurrentMovementState = movementType;
        }

        public void MainAttackTransition(string attack)
        {
          AnimTree.Set("parameters/MainhandState/"+ CurrentWeaponState
           +"/" + CurrentAttunement +"/conditions/" + PreviousAttack, false);

          AnimTree.Set("parameters/MainhandState/"+ CurrentWeaponState
           +"/" + CurrentAttunement +"/conditions/" + attack, true);


          PreviousAttack = attack;
        }

        public void UpdateAttunement(string attun)
        {
          AnimTree.Set("parameters/MainhandState/" + CurrentWeaponState + "/conditions/" + CurrentAttunement, false);
          CurrentAttunement = attun;
          AnimTree.Set("parameters/MainhandState/" + CurrentWeaponState + "/conditions/" + attun, true);
          //parameters/MainhandState/Sword/conditions/Fire
        }

        public void WeaponTransition(string weaponName)
        {
          AnimTree.Set("parameters/MovementState/conditions/" + CurrentWeaponState, false);
          AnimTree.Set("parameters/JumpState/conditions/" + CurrentWeaponState, false);
          AnimTree.Set("parameters/MainhandState/conditions/" + CurrentWeaponState, false);
          AnimTree.Set("parameters/MainhandState/" + CurrentWeaponState + "/conditions/" + CurrentAttunement, false);

          AnimTree.Set("parameters/MovementState/conditions/" + weaponName, true);  
          AnimTree.Set("parameters/JumpState/conditions/" + weaponName, true);
          AnimTree.Set("parameters/MainhandState/conditions/" + weaponName, true);
          AnimTree.Set("parameters/MainhandState/" + weaponName + "/conditions/" + CurrentAttunement, true);

          CurrentWeaponState = weaponName;
          //same for attack etc
        }

        public void BlendMovement(Vector2 blendPos)
        {
          //parameters/MovementState/Unarmed/Run/blend_position
          AnimTree.Set("parameters/MovementState/" + CurrentWeaponState + "/" + CurrentMovementState +"/blend_position", blendPos);
        }

        public void RequestOneShot(string oneShotName)
        {
          AnimTree.Set("parameters/" + oneShotName + "/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        }

        public void JumpType(string jumpType)
        {
          AnimTree.Set("parameters/JumpState/" + CurrentWeaponState + "/conditions/" + jumpType, true);
          //AnimTree.Set("parameters/JumpState/" + CurrentWeaponState +"/conditions/" + jumpType, false);
        }
    }
}
