using Game;
using Godot;
using System;

public partial class AIHealthBar : Sprite3D
{

	[Export] private TextureProgressBar bar;
	[Export] private double hideTimer = 2;
	private Node parent;
	private double timer;

	public override void _Ready()
	{
		parent = GetParent();
		
		if (parent is AI ai)
		{
			ai.HP.ValueChangedWithArgument += OnHpChanged;
			bar.MaxValue = ai.HP.MaxValue;
			bar.Value = bar.MaxValue;
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
        if ((parent as AI).LockOn.Visible == true) Show();
		else Hide();
		// SetPhysicsProcess(false);
		// timer = hideTimer;
    }


    public override void _PhysicsProcess(double delta)
    {
        timer -= delta;
		if (timer <= 0)
		{
			Hide();
			SetPhysicsProcess(false);
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
