using Assets.Core.Enums;
using UnityEngine;

public interface IDamageable
{
    public void TakeDamage(float damage, DamageType damageType);

}
