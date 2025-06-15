using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Entities.Behaviour;

public class LightningSpellController : MonoBehaviour, IProjectile
{
    private GameObject shooter;
    private IDamager weapon;
    private Rigidbody2D rigid;
    private Vector2 direction;

    [SerializeField] private GameObject stunEffectPrefab;
    [SerializeField] private float stunDuration = 0.5f;
    [SerializeField] private float Range = 10f;
    [SerializeField] private GameObject lightningSegmentPrefab;
    [SerializeField] private float jumpRangeDivider = 2f;
    [SerializeField] private int maxJumps = 5;
    [SerializeField] private LayerMask entityLayerMask;
    [SerializeField] private float defaultLength = 3f;

    private List<Entity> alreadyHit = new List<Entity>();

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

        var hit = new Hit
        {
            currentOrigin = shooter.transform.position,
            currentDirection = direction,
            currentRange = Range,
            hitOccurred = false
        };

        Entity firstTarget = FindTarget(hit.currentOrigin, hit.currentDirection, hit.currentRange);
        if(firstTarget == null)
        {
            Vector2 endPoint = (Vector2) shooter.transform.position + direction * defaultLength;
            CreateLightningSegment(shooter.transform.position, endPoint);
            return;
        }

        HandleHit(firstTarget, hit);
        for (int i = 0; i < maxJumps; i++)
        {
            Entity target = FindJumper(hit.currentOrigin, hit.currentDirection, hit.currentRange);
            if (target == null)
                break;

            HandleHit(target, hit);
        }
    }

    private void DelayedUnpause(GameObject target, float delay)
    {
        StartCoroutine(UnpauseAfterDelay(target, delay));
    }

    private System.Collections.IEnumerator UnpauseAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null)
        {
            IBehaviour behavior = target.GetComponent<IBehaviour>();
            if (behavior != null)
                behavior.Pause(false);

            Entity ent = target.GetComponent<Entity>();
            if (ent != null)
                ent.IsPaused = false;
        }
    }

    public void OnHit() { }

    public void AfterHit()
    {
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void CreateLightningSegment(Vector2 from, Vector2 to)
    {
        Vector2 midPoint = ( from + to ) / 2f;
        Vector2 dir = to - from;
        float distance = dir.magnitude;

        GameObject segment = Instantiate(lightningSegmentPrefab, midPoint, Quaternion.identity);
        segment.transform.right = dir.normalized;

        var sprite = segment.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.drawMode = SpriteDrawMode.Tiled; // Just in case
            sprite.size = new Vector2(distance * 10 , sprite.size.y);
        }

        Destroy(segment, 0.5f);
    }

    private Entity FindTarget(Vector2 origin, Vector2 direction, float range)
    {
        var rayhits = Physics2D.RaycastAll(origin, direction, range);
        if (rayhits != null)
        {
            foreach(var hit in rayhits)
            {
                Entity e = hit.collider.GetComponent<Entity>();
                if (e != null && !alreadyHit.Contains(e) && e.IsAlive && e.gameObject.GetInstanceID() != shooter.GetInstanceID())
                    return e;
            }
        }
        return null;
    }

    private Entity FindJumper(Vector2 origin, Vector2 direction, float range)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
        Entity closest = null;
        float closestDist = float.MaxValue;

        foreach (var h in hits)
        {
            Entity e = h.GetComponent<Entity>();
            if (e != null && !alreadyHit.Contains(e) && e.IsAlive && e.gameObject.GetInstanceID() != shooter.GetInstanceID())
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

    private void HandleHit(Entity target, Hit hit)
    {
        hit.hitOccurred = true;
        alreadyHit.Add(target);

        Vector2 targetPos = target.transform.position;
        CreateLightningSegment(hit.currentOrigin, targetPos);
        target.RecieveDamage(weapon);

        IBehaviour behavior = target.GetComponent<IBehaviour>();
        if (behavior != null)
        {
            behavior.Pause(true);
            target.IsPaused = true;

            if (stunEffectPrefab != null)
            {
                GameObject stunFx = Instantiate(stunEffectPrefab, target.transform.position, Quaternion.identity);
                Destroy(stunFx, stunDuration);
            }

            DelayedUnpause(target.gameObject, stunDuration);
        }

        hit.currentOrigin = targetPos;
        hit.currentDirection = ( targetPos - hit.currentOrigin ).normalized;
        hit.currentRange /= jumpRangeDivider;
    }
}
class Hit
{
    public Vector2 currentOrigin;
    public Vector2 currentDirection;
    public float currentRange;
    public bool hitOccurred;
}
