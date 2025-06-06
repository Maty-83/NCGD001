using Assets.Scripts;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class RaidTriggerTrapController : MonoBehaviour
{
    [SerializeField] private float TTL = 5000;
    [SerializeField] private List<GameObject> Spawners;
    [SerializeField] private GameObject EnemyPrefab;
    [SerializeField] private int EnemiesPerSpawn = 5;


    private List<GameObject> enemies = new List<GameObject>();
    private bool HasBeenActivated = false;
    private bool HasSpawned = false;

    private void Update()
    {
        if (HasBeenActivated && TTL > 0)
        {
            if (GameManager.Instance.BackgroundController.HasFinishedTransition && !HasSpawned)
            {
                SpawnEnemies();
                HasSpawned = true;
            }

            TTL -= Time.deltaTime;
            if (TTL <= 0)
            {
                

                GameManager.Instance.BackgroundController.SwitchToNormal();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (HasBeenActivated)
            return;

        if (collision.gameObject.GetComponent<PlayerController>() == null)
            return;

        GameManager.Instance.BackgroundController.SwitchToScary();
        HasBeenActivated = true;
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
                if(animator != null)
                {
                    animator.SetTrigger("Spawn");
                }
            }
        }
    }
}
