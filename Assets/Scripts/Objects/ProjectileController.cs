using Assets.Scripts;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Weapon;
using UnityEngine;

public class ProjectileController : MonoBehaviour, IObjectController, IProjectile
{
    public Rigidbody2D rBody;
    public float Speed { get; set; } = 1000f;
    public float TTL { get; set; } = 300;
    public Vector2 Direction { get; set; }

    private IDamager Weapon = new Gun();
    private GameObject Shooter = null;  

    private bool isPaused = false;

    public void Init(GameObject shooter, IDamager weapon, Rigidbody2D rigid, Vector2 direction)
    {
        Shooter = shooter;
        Direction = direction;
        Weapon = weapon;
        rBody = rigid;
    }

    public virtual void AfterHit()
    {
        Destroy();
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public virtual void Enter()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Exit()
    {
        throw new System.NotImplementedException();
    }

    public virtual void OnHit()
    {
        throw new System.NotImplementedException();
    }

    public void Pause()
    {
        isPaused = true;
    }

    public virtual void Play()
    {
        isPaused = false;
    }

    public virtual void Shoot()
    {
        if (!isPaused)
        {
            rBody.AddForce(( Direction.normalized * Speed ), ForceMode2D.Force);
            if (Mathf.Abs(rBody.linearVelocity.x) > Speed)
            {
                rBody.linearVelocity = new Vector2((
                    Mathf.Sign(rBody.linearVelocity.x) * Speed ),
                    rBody.linearVelocity.y);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TTL < 0)
        {
            Destroy();
        }
        TTL--;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Shooter == null)
            return;

        var collided = collision.gameObject;
        if (collided.gameObject == Shooter)
            return;

        var entity = collided.GetComponent <Entity>();
        if (entity)
        {
            entity.RecieveDamage(Weapon);
        }
        AfterHit();
    }
}
