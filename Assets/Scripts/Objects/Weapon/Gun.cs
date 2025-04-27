using Assets.Core.Enums;

namespace Assets.Scripts.Objects.Weapon
{
    public class Gun : IDamager
    {
        public DamageType Type { get; set; } = DamageType.Bullet;
        public float BaseDamage { get; set; } = 20;
        public bool DestroysProjectiles { get; set; } = false;
        public int ProjectileTeam { get; set; } = 1;
    }
}
