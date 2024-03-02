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
		[Export] private Player player;
		[Export] private PlayerAnimation Animation;
		[Export] private Stamina Stam;
		[Export] private InputBuffer Buffer;
		[Export] new PlayerMovement Movement;
		private string CurrentAttunement;
		private string CurrentWeaponName;
		
		private string bufferNextAction;
		private Vector2 vectorForState = Vector2.Zero;
		private string nextAction;
		
		private Dictionary<string, bool> msgHandleAttack = new()
		{
			{ "", true }
		};
		private Dictionary<string, Vector2> msgForState = new()
			{
				{ "", Vector2.Zero }
			};
		public async override void _Ready()
		{
			await ToSignal(GetParent(), "ready");
            state = initialState;
            player = Actor as Player;
            Movement = player.Movement;
            Attack = player.Attack;
			CurrentAttunement = player.CurrentAttunement;
			
            foreach (State c in GetChildren().Cast<State>())
            {
                c.Actor = Actor;
                c.Movement = Movement;
                c.Animation = Actor.Animation;
                c.Attack = Attack;
                c.Stam = Actor.Stam;

                if (Actor is Player && (c is PlayerRunState))
                    (c as PlayerRunState).SetCamera((Actor as Player).Camera);
                if (Actor is Player && (c is PlayerDodgeState))
                    (c as PlayerDodgeState).SetCamera((Actor as Player).Camera);
                if (Actor is Player && (c is PlayerAttackState))
                    (c as PlayerAttackState).SetCamera((Actor as Player).Camera);
            }

            state.Enter(Msg);
		}

		#region Combat stuff
		public override void HandleAttackInput(Dictionary<string, bool> Msg)
		{
			if (state is PlayerRestingState) return;
			if (state is PlayerStaggerState) return;
			if (state is PlayerParryingState) return;
			if (state is PlayerDeathState) return;
			if (state is PlayerPickUpState) return;
			if (state is PlayerDrinkState) return;
			//if (player.Consuming == true) return;

			if (Actor.HasParryingWeapon())
			{
				//if (CurrentOffhand is ParryingObject parry) parry.ActivateParryWindow(); 
				if (Msg.ContainsKey(Actions.Parry)) HandleParry();
			}
			if (Actor.HasBlockWeapon())
			{
				if (Msg.ContainsKey(Actions.Block)) HandleBlock(Msg[Actions.Block]);
			}
			if (player.HasWeapon())
			{
				if (player.IsBlocking() && player.CanCounter() == false) return;
				HandleLightAttack(Msg);
			}
		}

		public void HandleParry()
		{
			if (state is PlayerParryingState) return;
			if (state is PlayerAttackState) return;
			if (state is PlayerDodgeState) return;
			if (state is PlayerAirState) return;	
			if (Stam.HasEnough((player.CurrentOffhand as ParryingObject).StaminaConsumption) == false) return;
			
			TransitionTo(nameof(PlayerParryingState), Msg);
		}

		private bool bufferAttackInc = false;

		private void HandleLightAttack(Dictionary<string, bool> Msg)
		{
			if (Msg.Keys.ToArray()[0].Contains(Actions.LightAttack) == false) return;
			if (Stam.HasEnough(Actor.CurrentWeapon.LightAttackStamConsumption) == false) return;
			nextAction = Msg.Keys.ToArray()[0];
			
			//True means action pressed
			if (Msg.ContainsValue(true))
			{
				if (player.HasAimingWeapon())
				{
					BowAttack(nextAction);
				}
				else if (Buffer.IsEmpty())
				{	
					if (player.IsDodging() && Buffer.IsActivated())
					{
						Buffer.AddToBuffer(Actions.DodgeLightAttack);
					}
					else if (player.IsAttacking() && Buffer.IsActivated())
					{
						Buffer.AddToBuffer(Actions.LightAttack);
					}
					else if (bufferAttackInc || player.IsAttacking() == false) //if starting to attack, or ExecuteInputBuffer called
					{
						bufferAttackInc = false;
						
						msgForState.Clear();
						vectorForState.Y = Buffer.Chain;
					
						if (Movement._Sprinting) nextAction = Actions.SprintLightAttack;
						else if(player.IsBlocking()) nextAction = Actions.CounterLightAttack;

						msgForState[nextAction] = vectorForState;
						TransitionTo(nameof(PlayerAttackState), msgForState);
					}
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

			if (action == Actions.LightAttack)
			{
				Animation.Transition("Bow" + CurrentAttunement, Animations.Release);
				Animation.OneShot("Bow");
				(Actor.CurrentWeapon as Bow).Release();
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
			if (player.IsDodging()) return;
			if (pressed) Block();
			else BlockRelease();
		}

		private void Block()
		{
			if (player.IsBlocking()) return;
			if (player.IsAttacking()) return;
			CancelSprint();
			Walk();
			player.Block();
		}

		private void BlockRelease()
		{

			CancelWalk();
			player.BlockRelease();
		}

		#endregion

		#region Movement stuff

		public override void HandleMovementInput(Dictionary<string, Vector2> Msg)
		{
			if (state is PlayerRestingState) return;
			if (state is PlayerStaggerState) return;
			if (state is PlayerDodgeState) return;
			if (state is PlayerParryingState) return;
			if (state is PlayerDeathState) return;
			if (state is PlayerPickUpState) return;
			if (state is PlayerDrinkState) return;
			


			if (Actor.IsOnFloor() == false)
			{
				if ((state is PlayerAirState) == false) TransitionTo(nameof(PlayerAirState), Msg);
				return;
			}

			Move(Msg);
			
			if (state is PlayerAttackState) return;
			//if (player.Consuming == true) return;


			Sprint(Msg);
			Jump(Msg);
			Dodge(Msg);
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
				else TransitionTo(nameof(PlayerRunState), Msg);
			}
		}
		private void Sprint(Dictionary<string, Vector2> Msg)
		{
			if (player.Camera._AimOn) return;


			if (Msg.ContainsKey(Actions.Sprint) && player.IsBlocking() == false) 
			{
				if (Msg[Actions.Sprint] == Vector2.Zero) return;

				if (state.Anim == Animations.Sprint) return;
				//TransitionTo(nameof(PlayerRunState), Msg);
				player.Sprint();
			}

			if (Msg.ContainsKey(Actions.SprintRelease) && player.IsBlocking() == false)
			{
				//state.SetAnim(Animations.TransitionMovement, Animations.SprintRelease);
				//TransitionTo(nameof(PlayerRunState), Msg);
				GD.Print("HHHH");
				player.ReleaseSprint();
			}
		}
		private void Jump(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Jump)) 
			{
				player.ReleaseSprint(false);
				TransitionTo(nameof(PlayerAirState), Msg);
			}
		}
		private void Dodge(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Dodge) && Stam.HasEnough(Stam.DodgeConsumption)) 
			{
				BlockRelease();
				HandleCameraInput(Actions.AimCancel);
				TransitionTo(nameof(PlayerDodgeState), Msg);
			}
		}

		#endregion

		#region Camera stuff

		public void HandleCameraInput(string action)
		{
			Aim(action);
			LockOn(action);
		}

		private void LockOn(string action)
		{
			if (action == Actions.LockOn)
			{
				if (player.Camera.FindClosestTarget() == false) return;
				
				player.Camera.LockOnTarget();
			}
			if (action == Actions.LockOff)
			{
				player.Camera.ReleaseLockOn();
			}
		}

		private void Aim(string action)
		{
			if (player.IsDodging()) return;
			if (Actor.HasAimingWeapon() == false) return;
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
				player.Camera._AimOn = false;
				Attack.ReadyToShoot = false;
				CancelWalk();	

				if (Actor.CurrentWeapon is Bow bow)
				{
					bow.Release();
					Animation.AbortOneShot("Bow");
				}
			}
		}

		#endregion
		
		#region Signal/Event response
		

		public void OnParry()
		{
			
		}
		/*
		This heavily relies on animations finishing
		Probably better to use timers inside the states and send a signal when state is done
		We'll deal with it when we start having problems, so far so good 
		*/
		public void OnAnimationFinished(string anim)
		{

			GD.Print(anim);
			if (anim.Contains(Animations.Parry))
			{
				TransitionTo(nameof(PlayerRunState), Msg);
			}

			else if (anim.Contains("PickUp")) 
			{
				TransitionTo(nameof(PlayerRunState), Msg);
			
			}
			else if (anim.Contains("Drink")) TransitionTo(nameof(PlayerRunState), Msg);
			else if (anim.Contains("RestToStand")) TransitionTo(nameof(PlayerRunState), Msg);

			else if (anim.Contains(Animations.Stagger)) TransitionTo(nameof(PlayerRunState), Msg);

			else if (anim.Contains(Animations.AttackGeneral) && anim.Contains("Block") == false)
			{
				Buffer.Chain = 1;
				TransitionTo(nameof(PlayerRunState), msgForState);
			}
			else if (anim.Contains(Animations.Dodge))
			{
				if (Buffer.IsEmpty())
				{
					TransitionTo(nameof(PlayerRunState), msgForState);
				}
				else
				{
					ExecuteBufferInput(anim);
				}
			}

			else if (anim.Contains(Animations.DrawBow))
			{
				if (player.Camera._AimOn)
					Attack.ReadyToShoot = true;
			}

			else if (anim.Contains(Animations.Release))
			{
				if (player.Camera._AimOn)
					DrawBow();
			}
		}

		public void OnAttunementChanged(string attun)
		{
			CurrentAttunement = attun;
			(Animation as PlayerAnimation).UpdateAttunement(attun);
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
				CurrentWeaponName = "Unarmed";
				
			} 
			else 
			{
				CurrentWeaponName = weapon.Name;
			}


			Animation.CurrentWeaponState = CurrentWeaponName;

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
			if (weapon == null)
			{
				Animation.CurrentOffhandState = "Unarmed";
				Animation.OffhandBlend(0.0);
				//Animation.AnimTree.Set("parameters/OffhandBlend/blend_amount", 0.0);
				//Animation.OffhandBlend(0.0);
			}
			else
			{
				//Animation.Transition(Animations.TransitionOffhand, weapon.GetType().Name);
				//Animation.AnimTree.Set("parameters/OffhandBlend/blend_amount", 1.0);
				//Animation.OffhandTransition(weapon.GetType().Name);
				if (weapon is Shield)
				{
					Animation.CurrentOffhandState = "Shield";
					Animation.OffhandBlend(0.8);
				}
				// if (weapon is Shield)
				// {
				// 	Animation.OffhandBlend(1.0);
				// }
			}
		}


		#endregion

		public void ExecuteBufferInput(string anim)
		{
			Buffer.DeactivateBuffer();
			if (Buffer.IsEmpty()) return;

			state.GetInput(Vector2.Zero);
			msgHandleAttack.Clear();
			

			msgHandleAttack.Add(Buffer.Pop1(), true);
			//Buffer.DeactivateBuffer();
			bufferAttackInc = true;
			HandleAttackInput(msgHandleAttack);
		}
		
		#region Temporary functions

		private void CancelSprint()
		{
			if (state.Anim.Contains(Animations.Sprint)) player.Movement.WantsReleaseSprint();
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

		private async void DrawBow()
		{
			Animation.Transition("Bow"+CurrentAttunement, "Draw");
			Animation.OneShot("Bow");
			await ToSignal(GetTree().CreateTimer(0.6), SceneTreeTimer.SignalName.Timeout);
			(player.CurrentWeapon as Bow).Draw();
		}

		#endregion
	}  
}
