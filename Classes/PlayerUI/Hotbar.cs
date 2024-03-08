using Godot;
using System;
using System.Linq;
using Globals;

namespace Game
{
	[GlobalClass]
	public partial class Hotbar : Control
	{
		//Connected to inventory via OnScreen.cs
		[Signal] public delegate void HotbarUsedWithArgumentEventHandler(InventorySlot slot);

		[Export] Button btn;
		public GridContainer HotbarInventory;

		private InventorySlot[] slotsInHotbar = new InventorySlot[4];
		private InventorySlot currentSlotInHotBar;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready();
			SetPhysicsProcess(false);
			SetProcess(false);
		}

		public void MyReady()
		{
			int i = 0;

			//await ToSignal(GetParent(), "ready");
			GD.Print("Hotbar");

			foreach (InventorySlot slot in HotbarInventory.GetChildren().Cast<InventorySlot>())
			{
				slotsInHotbar[i] = slot;
				i++;
			}

			SetPhysicsProcess(true);
			SetProcess(true);
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			if (currentSlotInHotBar == null)
			{
				foreach (InventorySlot slot in slotsInHotbar)
				{
					if (slot.IsEmpty() == true) continue;

					currentSlotInHotBar = slot;
					btn.Icon = (Texture2D)slot.item.Texture;
					btn.Text = slot.item.Quantity.ToString();
				}
			}

			else
			{
				if (currentSlotInHotBar.IsEmpty())
					{
						Clear();
					}

				else
				{
					if (currentSlotInHotBar.item is Consumable)
						btn.Text = currentSlotInHotBar.item.Quantity.ToString();
					else btn.Text = "";
				}
				if (Input.IsActionJustPressed(Actions.UseItem))
				{
					UseHotBarItem();
				}
				
			}
		}

		public void Clear()
		{
			btn.Icon = null;
			currentSlotInHotBar = null;
			btn.Text = "";
		}

		public async void UseHotBarItem()
		{
			if (currentSlotInHotBar == null) return;
			if (currentSlotInHotBar.IsEmpty()) return;
			if (currentSlotInHotBar.item is Consumable == false) return;

			EmitSignal(SignalName.HotbarUsedWithArgument, currentSlotInHotBar);
			await ToSignal(GetTree().CreateTimer(0.1), SceneTreeTimer.SignalName.Timeout);
		}
	}
}
