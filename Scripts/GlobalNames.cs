using System.Collections.Generic;
using System.Data;
using Godot;

namespace Globals
{
    public class SoundEffects
    {
        public const string ShieldBlock = "ShieldBlock";
        public const string ArrowBodyImpact = "ArrowBodyImpact";
        public const string SwordFleshHit = "SwordFleshHit";
        public const string PickUpItem = "PickUpItem";
        public const string EquipItem = "EquipItem";
        public const string SwordAttack1 = "SwordAttack1";
        public const string SwordAttack2 = "SwordAttack2";
        public const string SwordAttack3 = "SwordAttack3";
        public const string BowDraw = "BowDraw";
    }
    public class Animations
    {
        public const string JumpRunning = "RunningJump";
        public const string JumpStand = "StandingJump";
        public const string TransitionMovement = "MovementTransition";
        public const string TransitionOffhand = "OffHandTransition";
        public const string Movement = "Movement";
        public const string Walk = "Walk";
        public const string Dodge = "Dodge";
        public const string Sprint = "Sprint";
        public const string Fall = "Fall";
        public const string SprintRelease = "SprintRelease";
        public const string Attack1 = "Attack1";
        public const string Attack2 = "Attack2";
        public const string Attack3 = "Attack3";
        public const string ComboAttack = "ComboAttack";
        public const string SprintLightAttack = "SprintLightAttack";
        public const string SprintHeavyAttack = "SprintHeavyAttack";
        public const string DodgeLightAttack = "DodgeLightAttack";
        public const string CounterAttack = "CounterAttack";
        public const string JumpAttack = "JumpAttack";
        public const string AttackGeneral = "Attack";
        public const string Block = "Block";
        public const string BlockRelease = "BlockRelease";
        public const string BlockedAttack = "BlockedAttack";
        public const string BlockHold = "BlockHold";

        public const string DrawBow = "Draw";

        public const string Release = "Release";

        public const string Stagger = "Stagger";
    }

    public class Attunements
    {
        public const string Fire = "Fire";
        public const string Lightning = "Lightning";
    }

    public class Actions
    {
        public const string LockOn = "LockOn";
        public const string LockOff = "LockOff";
        public const string NextRightTarget = "NextRightTarget";
        public const string NextLeftTarget = "NextLeftTarget";
        public const string Jump = "Jump";
        public const string LightAttack = "LightAttack";
        public const string DodgeLightAttack = "DodgeLightAttack";
        public const string HeavyAttack = "HeavyAttack";
        public const string SprintLightAttack = "SprintLightAttack";
        public const string CounterLightAttack = "CounterLightAttack";
        public const string Block = "Block";
        public const string Aim = "Aim";
        public const string AimCancel = "AimCancel";
        public const string Dodge = "Dodge";
        public const string Sprint = "Sprint";
        public const string SprintRelease = "SprintRelease";
        public const string DrawRightWeapon = "DrawRightWeapon";
        public const string ChangeAttunement = "ChangeAttunement";
        public const string PickUp = "PickUp";
        public const string MoveForward = "move_forward";
        public const string MoveLeft = "move_left";
        public const string MoveRight = "move_right";
        public const string MoveBack = "move_backwards";
    }

    public class Slots
    {
        public const string Passive = "PASSIVE";
        public const string Head = "HEAD";
        public const string Body = "BODY";
        public const string Legs = "LEGS";
        public const string RightHand = "RIGHT_HAND";
        public const string LeftHand = "LEFT_HAND";
        public const string Hotbar = "HOT_BAR";
    }

    public class WeaponTypes
    {
        public const string Melee1H = "MELEE_1H";
        public const string Melee2H = "MELEE_2H";
        public const string Ranged1H = "RANGED_1H";
        public const string Ranged2H = "RANGED_2H";
        public const string Magic1H = "Magic1H";
        public const string Magic2H = "Magic2H";
        public const string Offhand = "OFFHAND";
    }

    public class ConsumableTypes
    {
        public const string Heal = "HEAL";
    }
}