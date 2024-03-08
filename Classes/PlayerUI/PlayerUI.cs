using Game;
using Godot;
using System;

public partial class PlayerUI : Control
{
	public Player player;
	[Export] public OnScreen onScreen;
	[Export] public Debug debug;
	[Export] public Inventory inventory;
	public override void _Ready()
	{
		player = Owner as Player;
		if (player == null) QueueFree();
		GD.Print("PlayerUI");

		if (onScreen != null)
			onScreen.player = player;

		if (debug != null)
			debug.player = player;
		
		if (inventory != null)
			inventory.player = player;

		base._Ready();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
