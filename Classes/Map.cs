using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Game
{
    [GlobalClass]
    public partial class Map : Node3D
    {
        [Export] public Marker3D PlayerSpawnPos;
        [Export] private Node enemies;
        [Export] private Node checkpoints;

        public Player Player;

        public override void _Ready()
        {
            foreach (AI enemy in enemies.GetChildren().Cast<AI>())
            {
                enemy.ActorDeathWithArgument += OnAIDeath;
                enemy.spawnPosition = enemy.GlobalPosition;
            }

            foreach (Checkpoint checkpoint in checkpoints.GetChildren().Cast<Checkpoint>())
            {
                checkpoint.OnCheckPointVisitedWithArgument += OnCheckPointVisited;
            }

            if (Player != null)
            {
                Player.ActorDeathWithArgument += OnPlayerDeath;
            }
        }

        public async void OnAIDeath(Actor actor)
        {
            GD.Print(actor.Name + " Died");
            await ToSignal(GetTree().CreateTimer(6), SceneTreeTimer.SignalName.Timeout);
            if (actor is AI ai)
            {
                ai.SMachine.SetProcess(false);
                ai.SMachine.SetPhysicsProcess(false);
                actor.Visible = false;
                actor.SetCollisionLayerValue(2, false);
            }
            // actor.QueueFree();
        }

        public async void OnPlayerDeath(Actor player)
        {
            await ToSignal(GetTree().CreateTimer(5.5), SceneTreeTimer.SignalName.Timeout);
            ResetEnemies();
            if (Player.lastVisitedCheckpoint == null)
            {
                Player.GlobalPosition = PlayerSpawnPos.GlobalPosition;
            }

            else
            {
                Player.GlobalPosition = Player.lastVisitedCheckpoint.SpawnPos.GlobalPosition;
            }

            Player.SMachine.TransitionTo(nameof(PlayerRunState), null);
            Player.SMachine.SetProcess(true);
            Player.SMachine.SetPhysicsProcess(true);
            Player.SetPhysicsProcess(true);
            Player.SetProcess(true);
            Player.HP.Heal(Player.HP.MaxValue);
            Player.Dead = false;

           RespawnEnemies();
        }

        public void OnCheckPointVisited(Checkpoint checkpoint)
        {
            Player.VisitedCheckPoint(checkpoint);
            checkpoint.Visiting = true;
            ResetEnemies();
        }

        public void OnCheckPointLeft()
        {
            RespawnEnemies();
        }

        private void ResetEnemies()
        {
            foreach (AI enemy in enemies.GetChildren().Cast<AI>())
            {
                enemy.SMachine.SetProcess(false);
                enemy.SMachine.SetPhysicsProcess(false);
                enemy.HP.Heal(enemy.HP.MaxValue);
                enemy.GlobalPosition = enemy.spawnPosition;
                enemy.Visible = false;
            }
        }

        private void RespawnEnemies()
        {
            foreach (AI enemy in enemies.GetChildren().Cast<AI>())
            {
                enemy.SMachine.TransitionTo(nameof(AIRoamState));
                enemy.SMachine.target = null;
                enemy.target = null;
                enemy.SMachine.SetProcess(true);
                enemy.SMachine.SetPhysicsProcess(true);
                enemy.SetPhysicsProcess(true);
                enemy.SetProcess(true);
                enemy.Visible = true;
                enemy.Dead = false;
                enemy.SetCollisionLayerValue(2, true);
            }
        }

        public override void _ExitTree()
        {
            base._ExitTree();

            foreach (Checkpoint checkpoint in checkpoints.GetChildren().Cast<Checkpoint>())
            {
                checkpoint.OnCheckPointVisitedWithArgument -= OnCheckPointVisited;
            }
        }
    }
}