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
		[Export] new PlayerMovement Movement;
		private string CurrentAttunement;
		private string CurrentWeaponName;
		private Player player;
		private string bufferNextAction;

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
			

			player.AttunementChanged += OnAttunementChanged;
			player.WeaponChanged += OnWeaponChanged;
			player.OffhandChanged += OnOffhandChanged;

			Stam.StaminaChanged += OnStaminaChanged;
		}

		#region Combat stuff

		public override void HandleAttackInput(Dictionary<string, bool> Msg)
		{
			
			if (state is PlayerStaggerState) return;

			if (Actor.HasBlockWeapon())
			{
				if (Msg.ContainsKey(Actions.Block)) HandleBlock(Msg[Actions.Block]);
			}

			if (Actor.HasWeapon())
			{	
				if (player.IsBlocking() && player.CanCounter() == false) return;
				if (Msg.ContainsKey(Actions.AttackLight)) LightAttack(Msg[Actions.AttackLight]);
				else if (Msg.ContainsKey(Actions.DodgeAttackLight)) DodgeLightAttack(Msg[Actions.DodgeAttackLight]);
				else if (Msg.ContainsKey(Actions.AttackHeavy)) HeavyAttack(Msg[Actions.AttackHeavy]);
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

				if (Buffer.IsEmpty() && (state is PlayerAttackState || player.IsDodging()))
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

		private void DodgeLightAttack(bool pressed)
		{
			
			Dictionary<string, Vector2> msg = new()
			{
				{ Actions.DodgeAttackLight, Vector2.Zero }
			};

			if (pressed == true && Stam.HasEnough(Actor.CurrentWeapon.LightAttackStamConsumption))
			{
				// if (Actor.HasAimingWeapon())
				// {
				// 	BowAttack(Actions.DodgeAttackLight);
				// 	return;
				// }

				if (Buffer.IsEmpty() == false)
					{
						Buffer.Pop();
					}
				TransitionTo("PlayerAttackState", msg);
				
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
			if (player.IsDodging()) return;
			if (pressed) Block();
			else BlockRelease();
		}

		private void Block()
		{
			//Animation.Transition("Shield", Animations.Block);
			//Animation.Block();
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
			if (state is PlayerStaggerState) return;
			if (state is PlayerDodgeState) return;

			if (Actor.IsOnFloor() == false)
			{
				if ((state is PlayerAirState) == false) TransitionTo(nameof(PlayerAirState), Msg);
				return;
			}

			Move(Msg);

			if (state is PlayerAttackState) return;

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
				if (Msg.ContainsKey("Run"))
				{
					if (Msg["Run"] == Vector2.Zero) return;
				}

				//state.SetAnim(Animations.TransitionMovement, Animations.Sprint);
				if (state.Anim == Animations.Sprint) return;
				TransitionTo(nameof(PlayerRunState), Msg);
			}

			if (Msg.ContainsKey(Actions.SprintRelease) && player.IsBlocking() == false)
			{
				//state.SetAnim(Animations.TransitionMovement, Animations.SprintRelease);
				TransitionTo(nameof(PlayerRunState), Msg);
			}
		}
		private void Jump(Dictionary<string, Vector2> Msg)
		{
			if (Msg.ContainsKey(Actions.Jump)) 
			{
				CancelSprint();
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
			}
		}

		#endregion
		
		#region Signal/Event response
		
		public void OnStagger()
        {
            TransitionTo(nameof(PlayerStaggerState), null);
        }

		public void OnAnimationFinished(string anim)
		{
			//GD.Print(anim);
			Dictionary<string, Vector2> msg = new()
			{
				{ "input_dir", Vector2.Zero }
			};

			if (anim == Animations.Block) player.BlockHold();
			else if (anim.Contains(Animations.Stagger)) 
			{
				TransitionTo(nameof(PlayerRunState), Msg);
			}
			else if (anim.Contains(Animations.BlockedAttack) && player.IsBlocking()) 
				player.BlockHold();

			else if (anim.Contains(Animations.DrawBow))
			{
				if (player.Camera._AimOn)
				{
					Attack.ReadyToShoot = true;
				}
			}
			else if (anim.Contains(Animations.Release))
			{
				if (player.Camera._AimOn)
				{
					DrawBow();
				}
			}
			else if (anim.Contains(Animations.Dodge)) 
			{
				if (Buffer.IsEmpty()) TransitionTo(nameof(PlayerRunState),  msg);
				else ExecuteBufferInput(anim);
			}
			else if (anim.Contains(Animations.AttackGeneral) && Buffer.IsEmpty()) 
			{
				state.GetInput(Vector2.Zero);
				TransitionTo(nameof(PlayerRunState),  msg);
				Buffer.Chain = 1;
			}
			
			else ExecuteBufferInput(anim);
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


		#endregion

		public void ExecuteBufferInput(string anim)
		{
			//if (Buffer.IsEmpty()) return;
			if (anim.Contains(Animations.AttackGeneral))
			{
				if (Buffer.IsEmpty() == false)
				{
					Dictionary<string, bool> m = new()
					{
						{Actions.AttackLight, true}
					};
					HandleAttackInput(m);
				}
			}

			if (anim.Contains(Animations.Dodge))
			{
				bufferNextAction = Buffer.Pop().Keys.ToArray()[0];
				if (bufferNextAction.Contains(Actions.AttackLight))
					bufferNextAction = Actions.DodgeAttackLight;

				Dictionary<string, bool> m = new()
				{
					{bufferNextAction, true}
				};
				HandleAttackInput(m);
			}
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

		private void DrawBow()
		{
			Animation.Transition("Bow"+CurrentAttunement, "Draw");
			Animation.OneShot("Bow");
		}

		#endregion
	}  
}
