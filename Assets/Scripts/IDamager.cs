using UnityEngine;

public enum DamageType
{
    bullet,
    melee,
    fireMagic,
    iceMagic,
    earthMagic,
    windMagic
}

public interface IDamager
{
    public DamageType Type {  get; set; }
    public float BaseDamage { get; set; }
    public bool DestroysProjectiles { get; set; }
    public int ProjectileTeam { get; set; }//TODO: Figure out if this is necessary for filtering interactions.
}
