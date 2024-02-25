using Godot;
using System;
using System.Collections.Generic;

namespace Game
{

	public delegate void NotifyTargetChange(Actor target);
	[GlobalClass]
	public partial class LockOnComponent : Node3D
	{
		/*
		Doesn't matter if this is a child of the CameraComponent or not
		As long as the CameraComponent recognizes this component
		The CameraComponent will self inject to this
		*/

		public event NotifyTargetChange TargetChanged;

		public CameraComponent camera;

		private Actor target;

		private List<Actor> targetsInCenter = new();
		private List<Actor> targetsOnLeft = new();
		private List<Actor> targetsOnRight = new();

		private float dist;
		private float minDist;

		public Actor FetchClosestTarget()
		{
			target = SearchInList(targetsInCenter);
			if (target != null) return target;	
			target = SearchInList(targetsOnRight);
			if (target != null) return target;	
			target = SearchInList(targetsOnLeft);
			if (target != null) return target;	

			return null;
		}

		private Actor SearchInList(List<Actor> list)
		{
			Actor temp = null;

			if (list.Count >= 1)
			{
				temp = list[0];
				minDist = GlobalPosition.DistanceSquaredTo(temp.GlobalPosition);
			}

			foreach (Actor possibleTarget in list)
			{
				if (target == possibleTarget) continue;
				
				dist = GlobalPosition.DistanceSquaredTo(possibleTarget.GlobalPosition);
				if (dist < minDist)
				{
					temp = possibleTarget;
					minDist = dist;
				}
			}

			return temp;
		}

		public void FetchRightTarget()
		{
			Actor tempTarget = null;
			/*
			If the target is on the left side, first look for a closer one in the left side
			If we found the same target, look in the center
			If same or null, look in right
			*/
			if (targetsOnLeft.Contains(target))
				{
					tempTarget = SearchInList(targetsOnLeft);
					if (tempTarget == target)
					{
						tempTarget = SearchInList(targetsInCenter);
						if (tempTarget == target || tempTarget == null)
						{
							tempTarget = SearchInList(targetsOnRight);
						}
					}
				}

			/*
			If target is in the center, look in the center first for a closer target
			If not found, look in the right side
			*/
			else if (targetsInCenter.Contains(target))
				{
					GD.Print(target);
					tempTarget = SearchInList(targetsInCenter);
					GD.Print(tempTarget);
					if (tempTarget == target)
					{
						tempTarget = SearchInList(targetsOnRight);
					}
				}

			/*
			If target is in the right, look only in the right side for another target
			*/
			else if (targetsOnRight.Contains(target))
				{
					tempTarget = SearchInList(targetsOnRight);
				}
			
			if (tempTarget != null)
			{
				target = tempTarget;
				TargetChanged?.Invoke(target);
			}
		}

		public void FetchLeftTarget()
		{
			Actor tempTarget = null;
			/*
			If the target is on the right side, first look for a closer one in the right side
			If we found the same target, look in the center
			If same or null, look in left
			*/
			if (targetsOnRight.Contains(target))
				{
					tempTarget = SearchInList(targetsOnRight);
					if (tempTarget == target)
					{
						tempTarget = SearchInList(targetsInCenter);
						if (tempTarget == target || tempTarget == null)
						{
							tempTarget = SearchInList(targetsOnLeft);
						}
					}
				}

			/*
			If target is in the center, look in the center first for a closer target
			If not found, look in the left side
			*/
			else if (targetsInCenter.Contains(target))
				{
					GD.Print(target);
					tempTarget = SearchInList(targetsInCenter);
					GD.Print(tempTarget);
					if (tempTarget == target)
					{
						tempTarget = SearchInList(targetsOnLeft);
					}
				}

			/*
			If target is in the left, look only in the left side for another target
			*/

			else if (targetsOnLeft.Contains(target))
				{
					tempTarget = SearchInList(targetsOnLeft);
				}
			
			if (tempTarget != null)
			{
				target = tempTarget;
				TargetChanged?.Invoke(target);
			}
		}

		// public bool FindClosestTarget()
		// {
		// 	if (possibleTargets.Count >=1) 
		// 	{
		// 		Target = possibleTargets[0];
		// 		distToTarget = Position.DistanceSquaredTo(Target.Position);
		// 	}

		// 	foreach (Actor possibleTarget in possibleTargets)
		// 	{
		// 		tempDistance = Position.DistanceSquaredTo(possibleTarget.Position);
		// 		if (distToTarget > tempDistance)
		// 		{
		// 			distToTarget = tempDistance;
		// 			Target = possibleTarget;
		// 		}
		// 	}
			
		// 	return Target != null;
		// }


		private void OnRightAreaEntered(Actor body)
		{
			if (body is AI)
			{
				targetsOnRight.Add(body);
			}
		}

		private void OnLeftAreaEntered(Actor body)
		{
			if (body is AI)
			{
				targetsOnLeft.Add(body);
			}
		}

		private void OnCenterAreaEntered(Actor body)
		{
			if (body is AI)
			{
				targetsInCenter.Add(body);
			}
		}

		private void OnRightAreaLeft(Actor body)
		{
			if (targetsOnRight.Contains(body))
			{
				if (target == body) 
				{
					// if (!targetsInCenter.Contains(target) && !targetsOnLeft.Contains(target))
					// {
					// 	target = null;
					// 	TargetChanged?.Invoke(target);
					// }
				}
				targetsOnRight.Remove(body);
			}
		}

		private void OnLeftAreaLeft(Actor body)
		{
			if (targetsOnLeft.Contains(body))
			{
				if (target == body) 
				{
					// if (!targetsInCenter.Contains(target) && !targetsOnRight.Contains(target))
					// {
					// 	target = null;
					// 	TargetChanged?.Invoke(target);
					// }
				}
				targetsOnLeft.Remove(body);
			}
		}

		private void OnCenterAreaLeft(Actor body)
		{
			if (targetsInCenter.Contains(body))
			{
				if (target == body) 
				{
					// if (!targetsOnRight.Contains(target) && !targetsOnLeft.Contains(target))
					// {
					// 	target = null;
					// 	TargetChanged?.Invoke(target);
					// }
				}
				targetsInCenter.Remove(body);
			}
		}
	}
}
