using Game;
using Godot;
using System;

public partial class HeadRotationComponent : Node3D
{
	/*
	Leave for now
	*/
	[Export] private Skeleton3D skeleton;
	[Export] private int headIndex = 5;
	[Export] private LockOnComponent lockOnComponent;
	[Export] private Marker3D defaultTarget;
	private Actor target;
	private Vector3 defaultTargetPos;
	private Vector3 currentTargetPos;
	private Transform3D newPose;
	private Transform3D currentPose;
	private Transform3D defaultPose;
	[Export] private bool enabled = true;
	private float overrideAmount = 0.0f;

	// Called when the node enters the scene tree for the first time.
	public async override void _Ready()
	{
		
		await ToSignal(GetParent(), "ready");
		if (GetParent() is Player)
			if (lockOnComponent != null)
				lockOnComponent.TargetChanged += LookAtTarget;
		else if (GetParent() is AI)
		{

		}

		defaultPose = skeleton.GetBoneGlobalPose(headIndex);
	}

    private void LookAtTarget(Actor newTarget)
    {
        target = newTarget;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		skeleton.ClearBonesGlobalPoseOverride();

		GD.Print(target);
		if (target != null) 
		{
			
			currentTargetPos = currentTargetPos.Slerp(target.GlobalPosition, 0.02f);
			overrideAmount = Mathf.Lerp(overrideAmount, 0.8f, 0.02f);
			currentPose = skeleton.GetBoneGlobalPose(headIndex);

			newPose = currentPose.LookingAt(skeleton.ToLocal(currentTargetPos),
			Vector3.Up, true);

			skeleton.SetBoneGlobalPoseOverride(headIndex, newPose, overrideAmount, true);
		}

		else 
		{
			overrideAmount = Mathf.Lerp(overrideAmount, 0.0f, 0.01f);
			skeleton.SetBoneGlobalPoseOverride(headIndex, defaultPose, overrideAmount, true);
		}
		

		
	}
}
