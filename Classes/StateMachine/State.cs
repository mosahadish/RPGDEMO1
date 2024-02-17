using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game
{
    public abstract partial class State : Node
    {
        public Actor Actor;
        public Movement Movement;
        public Attack Attack;
        public Animate Animation;
        public Stamina Stam;
        public Vector2 InputDir = Vector2.Zero;

        public string AnimTransition;
        public string Anim;

        public string WeaponName = "";

       
        public override async void _Ready()
        {
            //await Task.Run(WaitForParent);
            await ToSignal(GetParent(), "ready");
        }

        // private async void WaitForParent()
        // {
        //     await ToSignal(GetParent(), "ready");
        // }

        public abstract void Update(double delta);

        public abstract void PhysicsUpdate(double delta);


        public abstract void Enter(Dictionary<string, Vector2> msg);

        public void GetInput(Vector2 inputDir)
        {
            InputDir = inputDir;
        }

        public void SetAnim(string transition, string anim)
        {
            AnimTransition = WeaponName+transition;
            Anim = WeaponName+anim;

            //nimation.Transition(AnimTransition, Anim);
        }

        public abstract void Exit();
    }
}