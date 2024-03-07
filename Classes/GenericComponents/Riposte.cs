using System.Collections.Generic;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Riposte : Node
    {
        public List<Actor> enemiesInRange;
        public Player Player;
        private Actor target;
        private double timer;
        private float riposteRange = 1.5f;

        public void SetTarget(Actor actor)
        {
            if (actor.CanBeRiposted == false) return;

            target = actor;
            SetPhysicsProcess(true);
        }

        public override void _PhysicsProcess(double delta)
        {
            GD.Print(target);
            if (target == null) SetPhysicsProcess(false);

            else if (target.Parried == false)
            {   
                target = null;
            }
            
            else
            {
                if (target.GlobalPosition.DistanceTo(Player.GlobalPosition) <= riposteRange)
                {
                    (Player.Animation as PlayerAnimation).CurrentAttack = "Riposte";
                    GD.Print("Riposte on");
                }
            }
        }

    }
}