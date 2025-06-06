using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts;

public class SakuraFallController : MonoBehaviour
{
    [Header("References")]
    public List<GameObject> LeafPrefabs;
    public Transform BackgroundLayer;
    public GameObject ParentLayer;

    [Header("Manual Bounds (World Units)")]
    public float BackgroundWidth = 20f;
    public float BackgroundHeight = 10f;

    [Header("Spawn Settings")]
    public float SpawnInterval = 0.5f;
    public int MinLeaves = 1;
    public int MaxLeaves = 5;
    public float MinHorizontalGap = 0.5f;

    [Header("Wind Settings")]
    public Vector2 GlobalWindRange = new Vector2(-1f, 1f);
    public float WindChangeInterval = 5f;

    public Vector2 CurrentWind { get; private set; }

    private float spawnTimer = 0f;
    private float windTimer = 0f;
    private int windDirection = 1;

    private float spawnMinX;
    private float spawnMaxX;
    private float spawnY;

    void Start()
    {
        UpdateWind();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        windTimer += Time.deltaTime;

        if (windTimer >= WindChangeInterval)
        {
            if (Random.Range(0, 2) == 1)
                windDirection *= -1;

            UpdateWind();
            windTimer = 0f;
        }

        if (spawnTimer >= SpawnInterval)
        {
            SpawnMultipleLeaves();
            spawnTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        CalculateSpawnBounds();
    }

    void CalculateSpawnBounds()
    {
        
        Vector3 pos = GameManager.Instance.Camera.transform.position;

        spawnMinX = pos.x - BackgroundWidth;
        spawnMaxX = pos.x + BackgroundWidth;
        spawnY = pos.y + BackgroundHeight * 2;
    }

    void UpdateWind()
    {
        float strength = Random.Range(GlobalWindRange.x, GlobalWindRange.y);
        CurrentWind = Vector2.right * strength * windDirection;
    }

    void SpawnMultipleLeaves()
    {
        int leafCount = Random.Range(MinLeaves, MaxLeaves + 1);
        List<float> usedPositions = new List<float>();

        for (int i = 0; i < leafCount; i++)
        {
            float x;
            int safety = 100;

            do
            {
                x = Random.Range(spawnMinX, spawnMaxX);
                safety--;
            }
            while (IsTooClose(x, usedPositions) && safety > 0);

            usedPositions.Add(x);

            Vector3 spawnPos = new Vector3(x, spawnY, 0f);
            GameObject newLeaf = Instantiate(LeafPrefabs[Random.Range(0, LeafPrefabs.Count)], spawnPos, Quaternion.identity);

            var windFollower = newLeaf.GetComponent<SakuraLeafController>();
            if (windFollower != null)
            {
                windFollower.SetController(this);
            }

            Rigidbody2D rb = newLeaf.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    bool IsTooClose(float x, List<float> positions)
    {
        foreach (var pos in positions)
        {
            if (Mathf.Abs(pos - x) < MinHorizontalGap)
                return true;
        }
        return false;
    }
}
