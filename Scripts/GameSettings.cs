using Godot;
using Globals;
using System.Collections.Generic;

namespace GameSettings
{
    public static class Settings
    {
        private static string CurrentDevice = "";
        public const string PS4InputMapName = "PS4 Controller"; //Godot's given name
        public const string PS4Controller = "Sony"; //Godot's action input's name
        
        public static Dictionary<string,string> InputActions = new();

        public static void SetCurrentDevice()
        {
            CurrentDevice = GetConnectedJoypad();
        }

        public static string GetCurrentDevice()
        {
            return CurrentDevice;
        }

        public static string GetBindForAction(string action)
        {
            if (InputActions.ContainsKey(action)) return InputActions[action];
            return "No bound input";
        }

        private static string GetConnectedJoypad()
        {
            Godot.Collections.Array<int> connectedJoypads = Input.GetConnectedJoypads();
            if (connectedJoypads.Count == 0) return "Keyboard";

            int connectedJoypad = connectedJoypads[0];
            if (Input.GetJoyName(connectedJoypad) == PS4InputMapName)
                return PS4Controller;

            return null;
        }

        public static void SetInputDeviceActions()
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
                    SetControllerBindings(action, eventAsText);
                }
            }
        }

        private static void SetControllerBindings(StringName action, string eventAsText)
        {
            string[] eventAsTextSplit;
            if (eventAsText.Contains(PS4Controller))
                    {
                        eventAsTextSplit = eventAsText.Split(",");
                        foreach (string split in eventAsTextSplit)
                        {
                            if (split.Contains(PS4Controller))
                            {
                                InputActions[action] = split.Split(" ")[2];
                            }
                        }
                    }
        }
    }
}