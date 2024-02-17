using System.Collections.Generic;
using System.Text;
using Globals;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class AudioHandler : Node3D
    {
        private AudioStreamPlayer3D[] audioStreamArray = new AudioStreamPlayer3D[8];
        private AudioStreamPlayer3D audio;


        public override void _Ready()
        {
            for (int i = 0; i < audioStreamArray.Length; i++)
            {
                audio = new();
                AddChild(audio);
                audioStreamArray[i] = audio;
                audio.DopplerTracking = AudioStreamPlayer3D.DopplerTrackingEnum.PhysicsStep;
            }
        }

        public void Play(string audioName)
        {
            audio = FindInactivePlayer();
            if (audio == null) return;                          

            audio.Stream = GameData.Instance.FetchSound(audioName);
            audio.Play();
        }

        private AudioStreamPlayer3D FindInactivePlayer()
        {
            foreach (AudioStreamPlayer3D stream in audioStreamArray)
                if (stream.Playing == false) return stream;

            return null;
        }
    }
}