using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game
{
    public abstract partial class AIState : Node
    {
        [Signal] public delegate void StateFinishedWithArgumentEventHandler(string stateName);
        public AI AIActor;
        public Movement Movement;
        public Attack Attack;
        public AnimateAI Animation;
        public Stamina Stam;
        public Vector2 InputDir = Vector2.Zero;

        public Player target;

        public string WeaponName = "";

       
        public override async void _Ready()
        {
            await ToSignal(GetParent(), "ready");
        }

        public abstract void Update(double delta);

        public abstract void PhysicsUpdate(double delta);


        public abstract void Enter(Player target);

        public void GetInput(Vector2 inputDir)
        {
            InputDir = inputDir;
        }

        public abstract void Exit();
    }
}