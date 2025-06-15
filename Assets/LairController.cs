using Assets.Scripts;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class LairController : BasicEnemyController
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private float SpawnRate;
    [SerializeField] private float TriggerDistance = 1f;
    [SerializeField] private List<GameObject> Spawners;
    [SerializeField] private int EnemiesPerSpawn = 5;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float FadeDuration = 1f;

    private List<GameObject> enemies = new List<GameObject>();
    private float timer = 10000f;

    // Fade variables
    private bool isFadingOut = false;
    private float fadeTimer = 0f;
    private bool pendingDestroy = false;
    private bool pendingDestroyFlag = false;
    private bool HasEntered = false;

    public override void OnDeath(bool destroy = true)
    {
        if (isFadingOut) return;

        isFadingOut = true;
        pendingDestroy = destroy;
        fadeTimer = 0f;
        GameManager.Instance.DestroyLair(gameObject);
    }

    protected new void Update()
    {
        base.Update();

        if (isFadingOut)
        {
            if (spriteRenderer != null)
            {
                fadeTimer += Time.deltaTime;
                float t = Mathf.Clamp01(fadeTimer / FadeDuration);
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = color;

                if (t >= 1f && !pendingDestroyFlag)
                {
                    pendingDestroyFlag = true;
                    base.OnDeath(pendingDestroy);
                    Destroy(gameObject);
                }
            }
            else
            {
                if (!pendingDestroyFlag)
                {
                    pendingDestroyFlag = true;
                    base.OnDeath(pendingDestroy);
                    Destroy(gameObject);
                }
            }

            return;
        }

        bool playerInRange = IsPlayerInRange();
        if (playerInRange)
        {
            timer += Time.deltaTime;

            if ((timer >= SpawnRate || !HasEntered) && IsSpawnerReady())
            {
                HasEntered = true;
                SpawnEnemies();
                timer = 0f;
            }
        }
        else
        {
            timer = 0f;
        }
    }

    private bool IsSpawnerReady()
    {
        if (Spawners == null || Spawners.Count == 0)
            return false;

        var firstSpawner = Spawners[0];
        Collider2D[] hits = Physics2D.OverlapCircleAll(firstSpawner.transform.position, 1f);

        foreach (var hit in hits)
        {
            if (hit.gameObject.CompareTag("Enemy"))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsPlayerInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TriggerDistance);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            if(hit.gameObject.GetComponent<PlayerController>() != null)
                return true;
        }

        return false;
    }

    private void SpawnEnemies()
    {
        if (Spawners == null || EnemiesPerSpawn <= 0)
            return;

        foreach (var spawn in Spawners)
        {
            var position = spawn.transform.position;
            for (int i = 0; i < EnemiesPerSpawn; i++)
            {
                var instance = Instantiate(EnemyPrefab);
                instance.transform.position = position;
                enemies.Add(instance);

                var animator = instance.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.SetTrigger("Spawn");
                }
            }
        }
    }
}