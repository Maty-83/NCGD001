using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects.ScriptableObjects;
using Assets.Scripts.Objects;

public class LightningSpellController : MonoBehaviour, IProjectile
{
    private GameObject shooter;
    private IDamager weapon;
    private Rigidbody2D rigid;
    private Vector2 direction;

    [SerializeField] private float jumpRangeDivider = 2f; // How much to reduce range each jump
    [SerializeField] private int maxJumps = 5; // Max number of jumps
    [SerializeField] private LayerMask entityLayerMask; // Set in inspector to your Entity layer
    [SerializeField] private float Range = 5f;

    private List<Entity> alreadyHit = new List<Entity>();
    private SpriteRenderer spriteRenderer;

    public void Init(GameObject shooter, IDamager weapon, Rigidbody2D rigid, Vector2 direction)
    {
        this.shooter = shooter;
        this.weapon = weapon;
        this.rigid = rigid;
        this.direction = direction.normalized;
    }

    public void Shoot()
    {
        alreadyHit.Clear();

        Vector2 currentOrigin = transform.position;
        Vector2 currentDirection = direction;
        float currentRange = Range;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("LightningSpellController requires SpriteRenderer with Tiled mode.");
            return;
        }

        for (int i = 0; i < maxJumps; i++)
        {
            Entity target = FindTarget(currentOrigin, currentDirection, currentRange);

            if (target == null)
                break;

            alreadyHit.Add(target);

            target.RecieveDamage(weapon);
            Vector2 targetPos = target.transform.position;
            Vector2 dirToTarget = targetPos - currentOrigin;
            float distance = dirToTarget.magnitude;

            transform.right = dirToTarget.normalized;
            transform.position = currentOrigin;
            spriteRenderer.size = new Vector2(distance, spriteRenderer.size.y);

            currentOrigin = targetPos;
            currentRange /= jumpRangeDivider;
        }

        AfterHit();
    }

    public void OnHit(){}

    public void AfterHit()
    {
        // Destroy lightning object after short time so it can display visually
        Destroy(gameObject, 0.1f);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private Entity FindTarget(Vector2 origin, Vector2 direction, float range)
    {
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, range, entityLayerMask);
        if (hit.collider != null)
        {
            Entity entity = hit.collider.GetComponent<Entity>();
            if (entity != null && !alreadyHit.Contains(entity) && entity.IsAlive)
            {
                return entity;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range, entityLayerMask);
        Entity closest = null;
        float closestDist = float.MaxValue;

        foreach (var h in hits)
        {
            Entity e = h.GetComponent<Entity>();
            if (e != null && !alreadyHit.Contains(e) && e.IsAlive && e.gameObject != shooter)
            {
                float dist = Vector2.Distance(origin, e.transform.position);
                if (dist < closestDist)
                {
                    closest = e;
                    closestDist = dist;
                }
            }
        }

        return closest;
    }
}
