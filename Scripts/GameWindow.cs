using Godot;

namespace Game
{
    [GlobalClass]
    public partial class GameWindow : Node
    {
        [Export] Map currentMapInstance;
        
        private PackedScene playerScene = (PackedScene)ResourceLoader.Load("res://Scenes/Models/Player/Player.tscn");
        private Player player;

        public override void _Ready()
        {
            player = (Player)playerScene.Instantiate();
            
            currentMapInstance.AddChild(player);
            player.GlobalPosition = currentMapInstance.PlayerSpawnPos.GlobalPosition;
        }
        
        public override void _Notification(int what)
        {
            base._Notification(what);
            if (what == NotificationWMCloseRequest)
                {
                    currentMapInstance.QueueFree();
                    GetTree().Quit();
                }

        }
    }
}