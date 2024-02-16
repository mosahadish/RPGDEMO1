using System;
using Godot;

namespace Game
{
    [GlobalClass]
    public partial class Projectile : RigidBody3D
    {
        [ExportCategory("Stats")]
        [Export] public float Damage;
        [Export] public float Speed;

        public Actor Spawner;


        [ExportCategory("Dependencies")]
        [Export] private Area3D hitArea;

        [Export] private Timer timer;

        private Vector3 direction;

        public override void _Ready()
        {
            SetCollisionLayerValue(3, true);
            if (Spawner is AI)
                SetCollisionMaskValue(2, false);
            if (Spawner is Player)
                SetCollisionMaskValue(1, false);
            if (hitArea != null)
            {
                hitArea.BodyEntered += OnBodyEnteredHitArea;
            }
            
            if(timer != null)
            {
                timer.Timeout += OnTimeOut;
                timer.Start(8);
            }
        }

        private void OnTimeOut()
        {
            QueueFree();
        }

        private void OnBodyEnteredHitArea(Node3D body)
        {
            GD.Print(body);
            if (body is Actor actor)
            {
                if (actor.HasMethod("OnHit"))
                {
                    actor.OnHit(Damage, GlobalPosition, "Projectile");
                    QueueFree();
                }
            }
        }

        public void SpawnProjectile(Actor spawner, Vector3 spawnPos, Vector3 targetPos)
        {
            direction = targetPos - spawnPos;
            Spawner = spawner;
            Spawner.GetParent().AddChild(this);
            
            Position = spawnPos;
            LinearVelocity = Speed * direction;

            LookAt(Transform.Origin + direction, Vector3.Up);
            RotateObjectLocal(Vector3.Up, Mathf.Pi/2);
        }
    }
}