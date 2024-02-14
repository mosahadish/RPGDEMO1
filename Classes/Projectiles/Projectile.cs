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

        private Vector3 direction;

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