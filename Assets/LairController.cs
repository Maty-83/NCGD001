using Assets.Scripts;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;

public class LairController : Entity
{
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private float SpawnRate;
    [SerializeField] private float TriggerDistance = 1f;
    [SerializeField] private List<GameObject> Spawners;
    [SerializeField] private int EnemiesPerSpawn = 5;

    private List<GameObject> enemies = new List<GameObject>();
    private float timer = 0f;

    public override void OnDeath(bool destroy = true)
    {
        for (int i = 0; i < GameManager.Instance.Lairs.Count; i++)
        {
            if(GameManager.Instance.Lairs[i].GetInstanceID() == gameObject.GetInstanceID())
                GameManager.Instance.Lairs.RemoveAt(i);
        }

        base.OnDeath(destroy);
    }

    protected new void Update()
    {
        base.Update();
        if (timer > SpawnRate && IsPlayerInRange())
        {
            SpawnEnemies();
            timer = 0f;
        }

        timer += Time.deltaTime;
    }

    private bool IsPlayerInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, TriggerDistance);
        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue;

            return hit.gameObject.GetComponent<PlayerController>() != null;
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
