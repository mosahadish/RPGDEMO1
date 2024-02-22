using Godot;
using Globals;

namespace Game
{
	[GlobalClass]
    public partial class RangeAI : AI, IRangeAttacker
    {
		private Projectile Proj;

        public void FinishAttacking()
        {
            _IsAttacking = false;
        }

        public bool IsAttacking()
        {
            return _IsAttacking;
        }

        public void RangeAttack1()
        {
			if ((CurrentWeapon is Bow) == false) return;
			Proj = GameData.Instance.FetchProjectile("FireArrow"); 
			(Animation as AnimateAI).Transition(Animations.Attack1);
			Proj.SpawnProjectile(this, CurrentWeapon.GlobalPosition, SMachine.target.LockOn.GlobalPosition);
            _IsAttacking = true;
        }

        public void RangeAttack2()
        {
            _IsAttacking = true;
        }

        public void RangeAttack3()
        {
            _IsAttacking = true;
        }
    }
}
