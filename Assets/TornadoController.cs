using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Entities;
using Assets.Scripts.Entities.Behaviour;

public class TornadoController : MonoBehaviour
{
    [SerializeField] public float TTL = 5f;
    [SerializeField] public float SuctionForce = 500f;
    [SerializeField] public float RotationForce = 200f;
    [SerializeField] public float ThrowForce = 1000f;
    [SerializeField] public float HoldDuration = 0.5f;


    private float leftLimit;
    private float rightLimit;
    private bool movingLeft;
    private float moveSpeed;

    // List of affected entities
    private Dictionary<Rigidbody2D, float> affectedEntities = new Dictionary<Rigidbody2D, float>();
    private List<Rigidbody2D> toAdd = new List<Rigidbody2D>();
    private List<Rigidbody2D> toRemove = new List<Rigidbody2D>();

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

        HandleRemoval();
        HandleAddition();
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
        var keys = new List<Rigidbody2D>(affectedEntities.Keys);

        foreach (var rb in keys)
        {
            float timeInside = affectedEntities[rb] + Time.deltaTime;
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

    private void HandleAddition()
    {
        foreach (var added in toAdd)
        {
            affectedEntities.Add(added, 0f);
        }
        toAdd.Clear();
    }

    private void HandleRemoval()
    {
        foreach (var removed in toRemove)
        {
            affectedEntities.Remove(removed);
        }
        toRemove.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb = null;
        var entity = collision.gameObject.GetComponent<Entity>();
        var airParticle = collision.gameObject.GetComponent<SakuraLeafController>();

        if (entity != null)
        {
            entity.IsPaused = true;
            IBehaviour behav = collision.gameObject.GetComponent<IBehaviour>();
            if (behav != null)
            {
                behav.Pause(false);
            }

            rb = collision.attachedRigidbody;
        }
        else if (airParticle != null)
        {
            rb = collision.attachedRigidbody;
        }

        if (rb != null && !affectedEntities.ContainsKey(rb))
        {
            toAdd.Add(rb);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Rigidbody2D rb = null;
        var entity = collision.gameObject.GetComponent<Entity>();
        var airParticle = collision.gameObject.GetComponent<SakuraLeafController>();

        if (entity != null)
        {
            entity.IsPaused = false;
            IBehaviour behav = collision.gameObject.GetComponent<IBehaviour>();
            if(behav != null)
            {
                behav.Pause(false);
            }

            rb = collision.attachedRigidbody;
        }
        else if(airParticle != null)
        {
            rb = collision.attachedRigidbody;
        }

        if (rb != null && affectedEntities.ContainsKey(rb))
        {
            toRemove.Remove(rb);
        }
    }
}
