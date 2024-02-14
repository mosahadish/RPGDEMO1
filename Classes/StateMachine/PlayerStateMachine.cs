using Godot;
using System;
using System.Collections.Generic;
using Globals;
using System.Linq;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class PlayerStateMachine : StateMachine
	{
		[ExportCategory("Dependencies")]
		[Export] PlayerAnimation Animation;
		[Export] Stamina Stam;
		[Export] InputBuffer Buffer;
		private string CurrentAttunement;
		private string CurrentWeaponName;

		public override void _Ready()
		{
			base._Ready();
			//Animation.AnimTree.Connect("AnimationFinished", new Callable(this, nameof(OnAnimationFinished)));
			(Actor as Player).AttunementChanged += OnAttunementChanged;
			(Actor as Player).WeaponChanged += OnWeaponChanged;
			(Actor as Player).OffhandChanged += OnOffhandChanged;
			Stam.StaminaChanged += OnStaminaChanged;
		}

		public override void HandleAttackInput(Dictionary<string, bool> Msg)
		{
			if (Actor._IsDodging) return;
			if (Actor.HasBlockWeapon())
			{
				if (Msg.ContainsKey(Actions.Block)) HandleBlock(Msg[Actions.Block]);
			}

			if (Actor.HasWeapon())
			{	
				if (Actor._IsBlocking) return; //change this later to add counter attack

				CancelSprint();

				if (Msg.ContainsKey(Actions.AttackLight)) LightAttack(Msg[Actions.AttackLight]);
				if (Msg.ContainsKey(Actions.AttackHeavy)) HeavyAttack(Msg[Actions.AttackHeavy]);
			}
		}

		private void LightAttack(bool pressed)
		{
			
			Dictionary<string, Vector2> msg = new()
			{
				{ Actions.AttackLight, Vector2.Zero }
			};

			if (pressed == true && Stam.HasEnough(Actor.CurrentWeapon.LightAttackStamConsumption))
			{
				if (Actor.HasAimingWeapon())
				{
					BowAttack(Actions.AttackLight);
					return;
				}

				if (Buffer.IsEmpty() && state is PlayerAttackState)
				{
					Buffer.AddToBuffer(Actions.AttackLight);
				}
				else
				{
					if (Buffer.IsEmpty() == false)
						{
							Buffer.Pop();
						}
					msg[Actions.AttackLight] = new Vector2(0, Buffer.Chain);
					TransitionTo("PlayerAttackState", msg);
				}
			}

			else
			{

			}
		}

		private void BowAttack(string action)
		{
			if ((Actor.CurrentWeapon is Bow) == false) return;
			if (Attack.ReadyToShoot == false) return;

			if (action == Actions.AttackLight)
			{
				Animation.Transition("Bow" + CurrentAttunement, Animations.Release);
				Animation.OneShot("Bow");
				(Attack as PlayerAttack).ReleaseArrow(CurrentAttunement);
				Attack.ReadyToShoot = false;
			}
		}

		private void HeavyAttack(bool pressed)
		{
			Dictionary<string, Vector2> msg = new()
			{
				{ "", Vector2.Zero }
			};

			if (pressed == true)
			{
				TransitionTo("PlayerAttackState", msg);
			}

			else
			{

			}
		}

		private void HandleBlock(bool pressed)
		{
			if (pressed) Block();
			else BlockRelease();
		}

		private void Block()
		{
			Actor._IsBlocking = true;

			Animation.Transition("Shield", Animations.Block);
			Walk();

			CancelSprint();

			Stam.Degen = true;
			Stam.Regen = false;
		}

		private void BlockRelease()
		{
			if (Actor._IsBlocking == false) return;

			Actor._IsBlocking = false;
			Animation.Transition("Shield", Animations.BlockRelease);
			CancelWalk();
				
			Stam.Degen = false;
			Stam.Regen = true;
		}

		public override void HandleMovementInput(Dictionary<string, Vector2> Msg)
		{
			
			if (state is PlayerDodgeState) return;

			if (Actor.IsOnFloor() == false)
			{
				if ((state is PlayerAirState) == false) TransitionTo("PlayerAirState", Msg);
				return;
			}

			Move(Msg);	

			if (state is PlayerAttackState) return;
			
			Jump(Msg);
			Dodge(Msg);
			Sprint(Msg);
		}

		private void Move(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey("Run"))
			{
				if (state is PlayerAttackState)
				{
					if (Msg["Run"] != Vector2.Zero) state.GetInput(Msg["Run"]);
					return;
				}
				if (state is PlayerRunState) state.GetInput(Msg["Run"]);
				else TransitionTo("PlayerRunState", Msg);
			}
		}
		private void Sprint(Dictionary<string, Vector2> Msg)
		{
			if ((Actor as Player).Camera._AimOn) return;

			if (Msg.ContainsKey(Actions.Sprint) && Actor._IsBlocking == false) 
			{
				//state.SetAnim(Animations.TransitionMovement, Animations.Sprint);
				if (state.Anim == Animations.Sprint) return;
				TransitionTo("PlayerRunState", Msg);
			}

			if (Msg.ContainsKey(Actions.SprintRelease) && Actor._IsBlocking == false)
			{
				//state.SetAnim(Animations.TransitionMovement, Animations.SprintRelease);
				TransitionTo("PlayerRunState", Msg);
			}
		}
		private void Jump(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Jump)) 
			{
				CancelSprint();
				TransitionTo("PlayerAirState", Msg);
			}
		}
		private void Dodge(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Dodge) && Stam.HasEnough(Stam.DodgeConsumption)) 
			{
				BlockRelease();
				HandleCameraInput(Actions.AimCancel);
				TransitionTo("PlayerDodgeState", Msg);
			}
		}

		public void HandleCameraInput(string action)
		{
			if (Actor.HasAimingWeapon() == false) return;
			if (Actor._IsDodging) return;
			
			if (action == Actions.Aim)
			{
				CancelSprint();

				(Actor as Player).Camera._AimOn = true;
				Movement.SetSpeed(Movement.WalkSpeed);
				Walk();	

				if (Actor.CurrentWeapon is Bow)
				{
					//Draw bow string
					DrawBow();
				}
			}

			if (action == Actions.AimCancel)
			{
				(Actor as Player).Camera._AimOn = false;
				Attack.ReadyToShoot = false;
				CancelWalk();	
			}
		}
		
		public void OnAnimationFinished(string anim)
		{
			GD.Print(anim);
			Dictionary<string, Vector2> msg = new()
			{
				{ "input_dir", Vector2.Zero }
			};

			if (anim.Contains(Animations.DrawBow))
			{
				if ((Actor as Player).Camera._AimOn)
				{
					Attack.ReadyToShoot = true;
				}
			}
			if (anim.Contains(Animations.Release))
			{
				if ((Actor as Player).Camera._AimOn)
				{
					DrawBow();
				}
			}
			if (anim.Contains(Animations.Dodge)) TransitionTo("PlayerRunState",  msg);
			if (anim.Contains(Animations.AttackGeneral) && Buffer.IsEmpty()) 
			{
				TransitionTo("PlayerRunState",  msg);
				Buffer.Chain = 1;
			}
			
			ExecuteBufferInput(anim);
		}

		public void ExecuteBufferInput(string anim)
		{
			if (anim.Contains(Animations.AttackGeneral))
			{
				// if (Buffer.IsEmpty())
				// 	TransitionTo("PlayerRunState",  msg);
				
				// msg[Buffer.Pop()[Actions.AttackLight] = true;
				if (Buffer.IsEmpty() == false)
				{
					Dictionary<string, bool> m = new()
					{
						{Actions.AttackLight, true}
					};
					HandleAttackInput(m);
				}
			}
		}

		public void OnAttunementChanged(string attun)
		{
			CurrentAttunement = attun;
			GD.Print(CurrentAttunement);
			if (Actor.HasWeapon()) Animation.Transition(Actor.CurrentWeapon.GetType().Name+"AttunementTransition", attun);	

		foreach (State s in GetChildren().Cast<State>())
			{	
				if (s is PlayerAttackState)
				{
					s.SetAnim(CurrentAttunement, Animations.Attack1);
				}
			}
		}

		public void OnStaminaChanged(float value)
		{
			if (value == 0)
			{
				CancelSprint();
			}
		}

		public void OnWeaponChanged(Weapon weapon)
		{
			// if (weapon.Name == "Sword") Animation.Transition("TypeTransition", WeaponTypes.Melee1H);
			if (weapon == null)
			{
				Animation.Transition("TypeTransition", "KARATE");
				CurrentWeaponName = "";
			} 
			else 
			{
				CurrentWeaponName = weapon.Name;
				Animation.Transition("TypeTransition", weapon.SubType);
				Animation.Transition(CurrentWeaponName+"AttunementTransition", CurrentAttunement);
			}
		
			Attack.CurrentWeapon = weapon;

			foreach (State s in GetChildren().Cast<State>())
			{	
				s.WeaponName = CurrentWeaponName;
				if (s is PlayerAttackState)
				{
					s.SetAnim(CurrentAttunement, Animations.Attack1);
				}
			}
			// //Current state should be Air or Run, can't switch during Attack/Dodge
			if (state is PlayerRunState) state.SetAnim(Animations.TransitionMovement, Animations.Movement);

			// if (state is PlayerAirState) state.SetAnim(Animations.TransitionMovement, Animations.Fall);
		}

		public void OnOffhandChanged(Weapon weapon)
		{
			GD.Print(weapon);
			if (weapon == null)
			{
				Animation.AnimTree.Set("parameters/OffhandBlend/blend_amount", 0.0);
			}
			else
			{
				Animation.Transition(Animations.TransitionOffhand, weapon.GetType().Name);
				Animation.AnimTree.Set("parameters/OffhandBlend/blend_amount", 1.0);
			}
		}


		//These functions are to avoid repeating lines
		private void CancelSprint()
		{
			if (state.Anim.Contains(Animations.Sprint)) Actor.Movement.WantsReleaseSprint();
		}

		private void Walk()
		{
			state.SetAnim(Animations.TransitionMovement, Animations.Walk);
			Animation.Transition(CurrentWeaponName+Animations.TransitionMovement, CurrentWeaponName+Animations.Walk);
			Movement.SetSpeed(Movement.WalkSpeed);
		}

		private void CancelWalk()
		{
			state.SetAnim(Animations.TransitionMovement, Animations.Movement);
			Animation.Transition(CurrentWeaponName+Animations.TransitionMovement, CurrentWeaponName+Animations.Movement);
			Movement.SetSpeed(Movement.Speed);
		}

		private void DrawBow()
		{
			Animation.Transition("Bow"+CurrentAttunement, "Draw");
			Animation.OneShot("Bow");
		}
	}  
}
