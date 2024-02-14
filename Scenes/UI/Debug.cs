using Game;
using Godot;
using System;

namespace Game
{
    public partial class Debug : Control
    {

        [Export] private Player player;
        [Export] private Label state;
        [Export] private Label attun;
        [Export] private Label aiming;
        [Export] private Label canShoot;

        private StateMachine stateMachine;

        public async override void _Ready()
        {
            player.AttunementChanged += OnAttunementChanged;
            await ToSignal(Owner, "ready");
            base._Ready();
            stateMachine = player.SMachine;
            attun.Text = "Attun: Fire";
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
            state.Text = stateMachine.state.Name;
            aiming.Text = "Aiming: "+ player.Camera._AimOn.ToString();
            canShoot.Text = "CanShoot: " + player.Attack.ReadyToShoot.ToString();
        }

        private void OnAttunementChanged(string new_attun)
        {
            attun.Text = "Attun: " + new_attun;
        }
    }
}

