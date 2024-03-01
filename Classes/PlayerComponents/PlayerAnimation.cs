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
    public int CurrentChain = 1;
    public string CurrentOffhandState = "Unarmed";
    public string CurrentAttunement = "Fire";
    public string JumpType = "Standing";
    public string PreviousAttack = "";
    public bool Blocking = false;
    public bool BlockedAttack = false;
    public bool Resting = false;

    public override void _Ready()
    {
      base._Ready();
    }

    public override void _Process(double delta)
    {
      base._Process(delta);
    }

    public override void Transition(string transitionType, string transitionName)
    {
      AnimTree.Set("parameters/" + transitionType + "/transition_request", transitionName);
    }


    public void MainAttack(string attack)
    {
      AnimTree.Set("parameters/MainhandState/" + CurrentWeaponState
        + "/" + CurrentAttunement + "/conditions/" + PreviousAttack, false);

      AnimTree.Set("parameters/MainhandState/" + CurrentWeaponState
        + "/" + CurrentAttunement + "/conditions/" + attack, true);

      PreviousAttack = attack;
      RequestOneShot("MainAttack");
    }

    public void Offhand(string action)
    {
      //advance_condition

    }

    public void UpdateAttunement(string attun)
    {
      CurrentAttunement = attun;
    }

    public void OffhandBlend(double val)
    {
      AnimTree.Set("parameters/OffhandBlend/blend_amount", val);
    }

    public void BlendMovement(Vector2 blendPos)
    {
      //parameters/MovementState/Unarmed/Run/blend_position
      AnimTree.Set("parameters/MovementState/" + CurrentWeaponState + "/" + CurrentMovementState + "/blend_position", blendPos);
    }

    public void RequestOneShot(string oneShotName)
    {
      AnimTree.Set("parameters/" + oneShotName + "/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }
  }
}
