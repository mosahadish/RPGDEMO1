using Godot;
using Globals;
using GameSettings;
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

        public override void _EnterTree()
        {
           base._EnterTree();
           //This should be handled when the game is launched, I think
           Settings.SetCurrentDevice();
           Settings.SetInputDeviceActions();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventKey)
            {
                
                if (InputMap.EventIsAction(@event, "DebugMode"))
                {
                    
                    if (@event.IsPressed()) 
                    {
                        Settings.Debug = !Settings.Debug;
                    }
                }
            }
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