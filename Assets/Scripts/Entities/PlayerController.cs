using Assets.Core.Enums;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Weapon;
using System.Collections.Generic;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;

public class PlayerController : Entity
{
    [Header("Movement Settings")]
    public float accelPerSec = 80f;
    public float maxMoveSpeed = 20f;
    public float jumpSpeed=20f;
    public float gravityNormal = 3f;
    public float gravityMultNoUpKey = 2f;
    public float maxFallSpeed = 30f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 2f;

    public GameObject projectile = null;
    public float minTimeBetweenBullets = 0.2f;
    private float curTimeBetweenBullets = 0;
    public float minTimeBetweenMelee = 0.4f;
    private float curTimeBetweenMelee = 0;


    [Header("Status field")]
    public BarController hpBarController;
    public BarController manaController;

    private Rigidbody2D rb;

    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;
    private bool isWatchingRight = false;

    private new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        Resistancies = new();
        Weapons = new() { {"Sword", new Sword() },
                          {"Gun", new Gun()} };

        hpBarController.SetValues(HP, true, 0, HP);
        manaController.SetValues(Mana, true, 0, Mana);

    }

    private new void Update()
    {
        base.Update();        
    }

    private void FixedUpdate()
    {       
        HandleInput();
    }

    private void HandleInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool left, right, up, shoot, melee;
        //Old trick: You want to get key inputs all at once so it isn't inconsistent.
        left = Input.GetKey(KeyCode.D);
        right = Input.GetKey(KeyCode.A);
        up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);
        //Turn into continuous and enable repeat attacks?
        shoot = Input.GetMouseButton(0);
        melee = Input.GetMouseButton(1);

        if (up)
        {
            if (isGrounded)
                OnJump();
            rb.gravityScale = gravityNormal;
        }
        else
        {
            rb.gravityScale =gravityNormal*gravityMultNoUpKey;
        }
        bool activelyMoving=false;
        if (right && !left)
        {
            isWatchingRight = false;
            OnMove(false);
            activelyMoving = true;
        }
        if (left && !right)
        {
            isWatchingRight = true;
            OnMove(true);
            activelyMoving = true;
        }

        OnMoveDrag(activelyMoving);

        curTimeBetweenBullets += Time.fixedDeltaTime;
        curTimeBetweenMelee += Time.fixedDeltaTime; 
        if (shoot)
        {
            if(curTimeBetweenBullets>minTimeBetweenBullets)
                OnShoot();
        }

        if(melee)
        {
            if(curTimeBetweenMelee>minTimeBetweenMelee)
                OnMelee();
        }
    }

    public void OnMoveDrag(bool activelyMoving)
    {
        if (activelyMoving)
        {        
            if (rb.linearVelocityX > maxMoveSpeed)
            {
                rb.linearVelocityX -= Mathf.Min(rb.linearVelocityX - maxMoveSpeed, accelPerSec * Time.fixedDeltaTime * 2);
            }
            else if (rb.linearVelocityX < -maxMoveSpeed)
            {
                rb.linearVelocityX -= Mathf.Max(rb.linearVelocityX + maxMoveSpeed, accelPerSec * Time.fixedDeltaTime * -2);
            }
        }
        else
        {
            if (rb.linearVelocityX > 0)
            {
                if(rb.linearVelocityX> accelPerSec * Time.fixedDeltaTime)
                {
                    rb.linearVelocityX -= accelPerSec * Time.fixedDeltaTime;
                }
                else rb.linearVelocityX = 0;
            }
            else if(rb.linearVelocityX < 0)
            {
                if(rb.linearVelocityX<-accelPerSec * Time.fixedDeltaTime)
                {
                    rb.linearVelocityX += accelPerSec * Time.fixedDeltaTime;
                }
                else rb.linearVelocityX = 0;
            }
        }
        if (rb.linearVelocityY < -maxFallSpeed)
        {
            rb.linearVelocityY=-maxFallSpeed;//Here we just set it since excess falling speeds contribute to hitbox mis-detections
        }
    }
    public void OnMove(bool toRight)
    {
        var dir = toRight ? 1 : -1;

        rb.linearVelocityX += dir * accelPerSec * Time.fixedDeltaTime;//We use fixed delta to be correct
    }
    
    public void OnJump()
    {
        rb.linearVelocityY=jumpSpeed;
    }

    public void OnShoot()
    {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var instance = Instantiate(projectile);
        instance.transform.position = gameObject.transform.position;
        var controller = instance.AddComponent<ProjectileController>();
        var dir = -1 * (gameObject.transform.position - mouse);

        controller.Init(gameObject,
            Weapons["Gun"],
            instance.GetComponent<Rigidbody2D>(),
            new Vector2(dir.x, dir.y));

        controller.Shoot();
        curTimeBetweenBullets=0;
    }

    public void OnMelee()
    {
        Vector2 dir = Vector2.left;
        if(isWatchingRight) 
            dir = Vector2.right;

        var hits = Physics2D.RaycastAll(transform.position, Vector2.right, 3f);
        Debug.DrawLine(transform.position, dir * 10f, Color.red);

        if(hits.Length != 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject)
                    continue;

                var entity = hit.rigidbody.gameObject.GetComponent<Entity>();
                if (entity && Weapons.ContainsKey("Sword"))
                {
                    entity.RecieveDamage(Weapons["Sword"]);
                }
            }
        }
        curTimeBetweenMelee = 0;
    }

    public override void RecieveDamage(IDamager weapon)
    {
        base.RecieveDamage(weapon);
        hpBarController.SetValues(HP, true);
    }
}
