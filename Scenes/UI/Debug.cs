using Game;
using GameSettings;
using Godot;

namespace Game
{
    public partial class Debug : Control
    {

        public Player player;
        [Export] private Label debugMode;
        [Export] private Label state;
        [Export] private Label aiming;
        [Export] private Label canShoot;
        [Export] private Label blocking;
        [Export] private Label speed;
        [Export] private Label staggerHP;
        [Export] private Label bufferLabel;
        [Export] private Label FPS;

        private StateMachine stateMachine;
        private InputBuffer inputBuffer;

        public async override void _Ready()
        {
            base._Ready();
            await ToSignal(Owner, "ready");
            if (player != null)
                {
                    await ToSignal(player, "ready");
                    stateMachine = player.SMachine;
                    inputBuffer = (stateMachine as PlayerStateMachine).Buffer;
                }     
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

            FPS.Text = "FPS: " + Engine.GetFramesPerSecond().ToString();
            state.Text = stateMachine.state.Name;
            aiming.Text = "Aiming: "+ player.Camera._AimOn.ToString();
            canShoot.Text = "CanShoot: " + player.Attack.ReadyToShoot.ToString();
            blocking.Text = "Block: " + player.IsBlocking().ToString();
            speed.Text = "Speed: " + player.Movement.CurrentSpeed.ToString();
            staggerHP.Text = "StaggerHP: " +player.staggerComp.CurrentValue.ToString();
            bufferLabel.Text = "Buffer Empty:" + inputBuffer.IsEmpty().ToString();
        }
    }
}

