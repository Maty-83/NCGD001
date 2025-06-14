using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Entities;

public class TornadoController : MonoBehaviour
{
    [SerializeField] public float TTL = 5f;

    private float leftLimit;
    private float rightLimit;
    private bool movingLeft;
    private float moveSpeed;

    // Tornado effect params
    public float SuctionForce = 500f;
    public float RotationForce = 200f;
    public float ThrowForce = 1000f;
    public float HoldDuration = 0.5f; // how long entity is sucked before thrown

    // List of affected entities
    private Dictionary<Rigidbody2D, float> affectedEntities = new Dictionary<Rigidbody2D, float>();

    public void InitTrap(float leftLimit, float rightLimit, bool startMovingLeft, float speed)
    {
        this.leftLimit = leftLimit;
        this.rightLimit = rightLimit;
        this.movingLeft = startMovingLeft;
        this.moveSpeed = speed;
    }

    void Update()
    {
        TTL -= Time.deltaTime;
        if (TTL < 0)
            Destroy(gameObject);

        MoveTornado();
        UpdateAffectedEntities();
    }

    private void MoveTornado()
    {
        float moveDelta = moveSpeed * Time.deltaTime;

        Vector3 position = transform.position;

        if (movingLeft)
        {
            position.x -= moveDelta;
            if (position.x <= leftLimit)
            {
                position.x = leftLimit;
                movingLeft = false;
            }
        }
        else
        {
            position.x += moveDelta;
            if (position.x >= rightLimit)
            {
                position.x = rightLimit;
                movingLeft = true;
            }
        }

        transform.position = position;
    }

    private void UpdateAffectedEntities()
    {
        List<Rigidbody2D> toRelease = new List<Rigidbody2D>();

        foreach (var entry in affectedEntities)
        {
            Rigidbody2D rb = entry.Key;
            float timeInside = entry.Value + Time.deltaTime;
            affectedEntities[rb] = timeInside;

            Vector2 tornadoCenter = transform.position;
            Vector2 entityPos = rb.worldCenterOfMass;

            Vector2 toCenter = ( tornadoCenter - entityPos ).normalized;
            rb.AddForce(toCenter * SuctionForce, ForceMode2D.Force);

            Vector2 tangent = new Vector2(-toCenter.y, toCenter.x);
            rb.AddForce(tangent * RotationForce, ForceMode2D.Force);

            if (timeInside >= HoldDuration)
            {
                Vector2 throwDirection = ( entityPos - tornadoCenter ).normalized;
                throwDirection.y = Mathf.Max(throwDirection.y, 0.3f); 

                rb.AddForce(throwDirection.normalized * ThrowForce, ForceMode2D.Force);

                toRelease.Add(rb);
            }
        }

        foreach (var rb in toRelease)
        {
            affectedEntities.Remove(rb);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            entity.IsPaused = true;
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null && !affectedEntities.ContainsKey(rb))
            {
                affectedEntities.Add(rb, 0f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var entity = collision.GetComponent<Entity>();
        if (entity != null)
        {
            entity.IsPaused = false;
            Rigidbody2D rb = collision.attachedRigidbody;
            if (rb != null && affectedEntities.ContainsKey(rb))
            {
                affectedEntities.Remove(rb);
            }
        }
    }
}
