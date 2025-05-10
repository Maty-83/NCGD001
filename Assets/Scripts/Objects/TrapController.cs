using Assets.Core.Enums;
using Assets.Scripts.Entities;
using UnityEngine;

public class TrapController : MonoBehaviour, IDamager
{
    [SerializeField] private DamageType DamageType = DamageType.Earth;
    [SerializeField] private float Damage = 10;
    [SerializeField] private int DamageTicks = 20;
    public DamageType Type { get { return DamageType; } set { DamageType = value; } }
    public float BaseDamage { get { return Damage; } set { Damage = value; } }
    public bool DestroysProjectiles { get; set; } = true;
    public int ProjectileTeam { get; set; } = 2;


    private int actualTicks = 0;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        actualTicks = DamageTicks;
        DealDamage(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (actualTicks == 0)
        {
            DealDamage(collision);
            actualTicks = DamageTicks;
        }
        actualTicks--;
    }

    private void DealDamage(Collider2D collision)
    {
        var collided = collision.gameObject;
        var entity = collided.GetComponent<PlayerController>();
        if (entity)
        {
            entity.RecieveDamage(this);
        }
    }
}
