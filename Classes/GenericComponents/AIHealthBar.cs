using Game;
using Godot;
using System;

namespace Game
{
	public partial class AIHealthBar : Sprite3D
	{

	[Export] private TextureProgressBar bar;
	[Export] private double hideTimer;
	private Node parent;
	private double timer;
	private bool lockedOn;

	public override void _Ready()
	{
		parent = GetParent();
		
		if (parent is AI ai)
		{
			bar.MaxValue = ai.HP.MaxValue;
			bar.Value = bar.MaxValue;
			
			ai.HP.ValueChangedWithArgument += OnHpChanged;

			if (ai.LockOn != null)
			{
				ai.LockOn.VisibilityChanged += OnLockOn;
			}
		}
		SetPhysicsProcess(false);
		timer = hideTimer;
		Hide();
	}

    private void OnLockOn()
    {
        if ((parent as AI).LockOn.Visible == true) 
		{
			Show();
			timer = hideTimer;
			lockedOn = true;
		}
		else 
		{
			Hide();
			lockedOn = false;
		}
    }


    public override void _PhysicsProcess(double delta)
    {
        if (lockedOn == false)
		{
			timer -= delta;
			if (timer <= 0)
			{
				Hide();
				SetPhysicsProcess(false);
			}
		}
    }

    private void OnHpChanged(float val)
    {
        bar.Value = val;

		if (Visible == false)
		{
			timer = hideTimer;
			SetPhysicsProcess(true);
			Show();
		}
    }

    public override void _ExitTree()
    {
        if (parent is AI ai)
		{
			ai.HP.ValueChangedWithArgument -= OnHpChanged;
			if (ai.LockOn != null)
			{
				ai.LockOn.VisibilityChanged -= OnLockOn;
			}
		}
    }
	}
}