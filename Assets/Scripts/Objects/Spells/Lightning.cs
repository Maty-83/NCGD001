using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects.ScriptableObjects;
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

        Vector2 currentOrigin = shooter.transform.position;
        Vector2 currentDirection = direction;
        float currentRange = Range;

        bool hitOccurred = false;

        for (int i = 0; i < maxJumps; i++)
        {
            Entity target = FindTarget(currentOrigin, currentDirection, currentRange);

            if (target == null)
                break;

            hitOccurred = true;

            alreadyHit.Add(target);

            Vector2 targetPos = target.transform.position;
            CreateLightningSegment(currentOrigin, targetPos);

            target.RecieveDamage(weapon);

            IBehaviour behavior = target.GetComponent<IBehaviour>();
            if (behavior != null)
            {
                behavior.Pause(true);

                if (stunEffectPrefab != null)
                {
                    GameObject stunFx = Instantiate(stunEffectPrefab, target.transform.position, Quaternion.identity);
                    Destroy(stunFx, stunDuration);
                }

                DelayedUnpause(target.gameObject, stunDuration);
            }

            currentOrigin = targetPos;
            currentDirection = ( targetPos - currentOrigin ).normalized;
            currentRange /= jumpRangeDivider;
        }

        // No hit? Show a default lightning
        if (!hitOccurred)
        {
            Vector2 endPoint = (Vector2) shooter.transform.position + direction * defaultLength;
            CreateLightningSegment(shooter.transform.position, endPoint);
        }

        AfterHit();
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
        }
    }

    public void OnHit() { }

    public void AfterHit()
    {
        Destroy(gameObject, 0.1f);
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

        Destroy(segment, 1f);
    }

    private Entity FindTarget(Vector2 origin, Vector2 direction, float range)
    {
        var rayhits = Physics2D.RaycastAll(origin, direction, range);
        if (rayhits != null)
        {
            foreach(var hit in rayhits)
            {
                Entity e = hit.collider.GetComponent<Entity>();
                if (e != null && !alreadyHit.Contains(e) && e.IsAlive && e.gameObject != shooter)
                    return e;
            }
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, range);
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
