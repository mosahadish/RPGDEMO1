using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Game
{
	[GlobalClass]
	public partial class InputBuffer : Node
	{
        public int Chain = 1;
        
        private bool bufferNextAttack = false;
        private int maxChain = 3;
        private float timeNow = 0;
        private float lastUpdateTime = 0;
        private Stack<Dictionary<string,float>> buffer;


        public override void _Ready()
        {
            buffer = new();
        }   

        public override void _PhysicsProcess(double delta)
        {
            timeNow += 1.0f/60.0f; //Convert to seconds, PhysicsProcess runs 60 times per sec
            if (Chain !=1 && (timeNow-lastUpdateTime) > 2)
            {
                Chain = 1;
            }

            if (IsEmpty() == false)
            {
                if (timeNow - buffer.First()["Time"] > 2)
                {
                    buffer.Clear();
                }
            }
        }

        public void AddToBuffer(string action)
        {
            if (IsEmpty() && IsActivated())
            {
                lastUpdateTime = timeNow;
                Chain +=1;
                Dictionary<string, float> msg = new()
			    {
				    { action, Chain },
                    { "Time", lastUpdateTime}
			    };
                
                if (Chain > maxChain) Chain = 1;

                buffer.Push(msg);
                DeactivateBuffer();
            }
        }

        public bool IsActivated()
        {
            return bufferNextAttack;
        }

        public void ActivateBuffer()
        {
            bufferNextAttack = true;
        }

        public void DeactivateBuffer()
        {
            bufferNextAttack = false;
        }

        public bool IsEmpty()
        {
            return buffer.Count == 0;
        }

        public Dictionary<string, float> Pop()
        {
            return buffer.Pop();
        }

    }
}