using Godot;
using Globals;
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
            DetectTypeOfInputDevice();
            SetInputDeviceActions();
        }

        private static void DetectTypeOfInputDevice()
        {
            CurrentInputDevice.CurrentDevice = GetConnectedJoypad();
        }

        private static string GetConnectedJoypad()
        {
            Godot.Collections.Array<int> connectedJoypads = Input.GetConnectedJoypads();
            if (connectedJoypads.Count == 0) return "Keyboard";

            int connectedJoypad = connectedJoypads[0];
            if (Input.GetJoyName(connectedJoypad) == CurrentInputDevice.PS4InputMapName)
                return CurrentInputDevice.PS4Controller;

            return null;
        }

        private static void SetInputDeviceActions()
        {
            Godot.Collections.Array<StringName> inputActions = InputMap.GetActions();
            Godot.Collections.Array<InputEvent> eventsPerAction;
            string eventAsText;
            
            //This works only for controllers afaik
            foreach (StringName action in inputActions)
            {
                eventsPerAction = InputMap.ActionGetEvents(action);
                foreach (InputEvent ev in eventsPerAction)
                {
                    eventAsText = ev.AsText();
                    GetControllerInputBtn(action, eventAsText);
                }
            }
        }

        private static void GetControllerInputBtn(StringName action, string eventAsText)
        {
            string[] eventAsTextSplit;
            if (eventAsText.Contains(CurrentInputDevice.PS4Controller))
                    {
                        eventAsTextSplit = eventAsText.Split(",");
                        foreach (string split in eventAsTextSplit)
                        {
                            if (split.Contains(CurrentInputDevice.PS4Controller))
                            {
                                CurrentInputDevice.InputActions[action] = split.Split(" ")[2];
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