using Assets.Core.Enums;
using Assets.Scripts.Entities;
using UnityEngine;

public class TrapController : MonoBehaviour, IDamager
{
    public DamageType Type { get; set; } = DamageType.Earth;
    public float BaseDamage { get; set; } = 10;
    public bool DestroysProjectiles { get; set; } = true;
    public int ProjectileTeam { get; set; } = 2;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collided = collision.gameObject;
        var entity = collided.GetComponent<PlayerController>();
        if (entity)
        {
            entity.RecieveDamage(this);
        }
    }
}
