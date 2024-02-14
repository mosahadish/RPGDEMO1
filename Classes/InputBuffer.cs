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
        public bool BufferNextAttack = false;

        public int Chain = 1;


        private int maxChain = 3;
        private float TimeNow = 0;
        private float LastUpdateTime = 0;
        private Stack<Dictionary<string,float>> buffer = new();


        public override void _Ready()
        {
            
        }

        public override void _PhysicsProcess(double delta)
        {
            TimeNow += 1.0f/60.0f; //Convert to seconds, PhysicsProcess runs 60 times per sec
            if (Chain !=1 && (TimeNow-LastUpdateTime) > 2)
            {
                Chain = 1;
            }

            if (IsEmpty() == false)
            {
                if (TimeNow - buffer.First()["Time"] > 2)
                {
                    buffer.Clear();
                }
            }
        }

        public void AddToBuffer(string action)
        {
            if (IsEmpty() && IsActivated())
            {
                LastUpdateTime = TimeNow;
                Chain +=1;
                Dictionary<string, float> msg = new()
			    {
				    { action, Chain },
                    { "Time", LastUpdateTime}
			    };
                
                if (Chain > maxChain) Chain = 1;

                buffer.Push(msg);
                DeactivateBuffer();
            }
        }


        public bool IsActivated()
        {
            return BufferNextAttack;
        }

        public void ActivateBuffer()
        {
            BufferNextAttack = true;
        }

        public void DeactivateBuffer()
        {
            BufferNextAttack = false;
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