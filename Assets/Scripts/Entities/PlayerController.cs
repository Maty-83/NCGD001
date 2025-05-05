using Assets.Core.Enums;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Weapon;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity
{
    [Header("Movement Settings")]
    public float moveSpeed = 30f;
    public float moveSpeedCap = 100f;
    public float jumpForce = 300f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.4f;

    public GameObject projectile = null;

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
        HandleInput();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (isGrounded)
                OnJump();
        }

        if (!isGrounded && rb.linearVelocityY == 0f)
        {
            return;
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                isWatchingRight = false;
                OnMove(false);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                isWatchingRight = true;
                OnMove(true);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnShoot();
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnMelee();
        }
    }


    public void OnMove(bool toRight)
    {
        var dir = toRight ? 1 : -1;

        rb.AddForce(new Vector2(dir * moveSpeed, 0), ForceMode2D.Force);

        if (Mathf.Abs(rb.linearVelocity.x) > moveSpeed)
        {
            rb.linearVelocity = new Vector2((Mathf.Sign(rb.linearVelocity.x) * moveSpeed), rb.linearVelocity.y);
        }
    }
    
    public void OnJump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
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
    }

    public override void RecieveDamage(IDamager weapon)
    {
        base.RecieveDamage(weapon);
        hpBarController.SetValues(HP, true);
    }
}
