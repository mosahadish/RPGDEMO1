using Game;
using GameSettings;
using Godot;
using System;

namespace Game
{
    public partial class Debug : Control
    {

        [Export] private Player player;
        [Export] private InputBuffer inputBuffer;
        [Export] private Label debugMode;
        [Export] private Label state;
        [Export] private Label aiming;
        [Export] private Label canShoot;
        [Export] private Label blocking;
        [Export] private Label speed;
        [Export] private Label staggerHP;
        [Export] private Label bufferLabel;

        private StateMachine stateMachine;

        public async override void _Ready()
        {
            await ToSignal(Owner, "ready");
            base._Ready();
            stateMachine = player.SMachine;
        }

        public override void _Process(double delta)
        {
            if (Settings.Debug == true)
            {
                Show();
            }
            else
            {   
                Hide();
            }
        
            state.Text = stateMachine.state.Name;
            aiming.Text = "Aiming: "+ player.Camera._AimOn.ToString();
            canShoot.Text = "CanShoot: " + player.Attack.ReadyToShoot.ToString();
            blocking.Text = "Block: " + player.IsBlocking().ToString();
            speed.Text = "Speed: " + player.Movement.CurrentSpeed.ToString();
            staggerHP.Text = "StaggerHP: " +player.staggerComp.CurrentValue.ToString();
            bufferLabel.Text = inputBuffer.IsEmpty().ToString();
        }
    }
}

