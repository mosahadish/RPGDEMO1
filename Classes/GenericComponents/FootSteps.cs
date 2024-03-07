using Godot;

namespace Game
{
    [GlobalClass]
    public partial class FootSteps : AudioStreamPlayer3D
    {
        [Export] private Actor actor;
        [Export] private Skeleton3D skeleton;
        [Export] private BoneAttachment3D rightFoot;
        [Export] private BoneAttachment3D leftFoot;
        [Export] private int leftFootIdx;
        [Export] private int rightFootIdx;

        [Export] private RayCast3D leftRay;
        [Export] private RayCast3D rightRay;

        private NodePath pathToSkeleton;
        private double timer = 0.1;
        public override void _Ready()
        {
            if (skeleton != null)
            {
                pathToSkeleton = skeleton.GetPath();
                if (rightFoot != null && leftFoot != null)
                {
                    rightFoot.SetUseExternalSkeleton(true);
                    leftFoot.SetUseExternalSkeleton(true);
                
                    rightFoot.SetExternalSkeleton(pathToSkeleton);
                    leftFoot.SetExternalSkeleton(pathToSkeleton);

                    rightFoot.BoneIdx = rightFootIdx;
                    leftFoot.BoneIdx = leftFootIdx;
                }
            }
        }
        public override void _PhysicsProcess(double delta)
        {
            if (actor.Velocity.IsEqualApprox(Vector3.Zero) == false && actor.IsOnFloor()) 
            {
                if (rightRay.IsColliding() || leftRay.IsColliding())
                {
                    if (Playing == false)
                        {
                            Play();
                        }
                }
                else Stop();
            }
            else 
            {
                Stop();
            }
        }
    }   
}