using Assets.Core.Enums;

namespace Assets.Scripts.Objects
{
    public class Sword : IDamager
    {
        public DamageType Type { get; set; } = DamageType.Melee;
        public float BaseDamage { get; set; } = 10;
        public bool DestroysProjectiles { get; set; } = true;
        public int ProjectileTeam { get; set; } = 0;
    }
}
