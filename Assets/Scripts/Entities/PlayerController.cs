using Assets.Core.Enums;
using Assets.Helpers.Enums;
using Assets.Scripts;
using Assets.Scripts.Entities;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Weapon;
using System.Collections.Generic;
using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : Entity
{
    [Header("Movement Settings")]
    public float accelPerSec = 80f;
    public float maxMoveSpeed = 20f;
    public float jumpSpeed=20f;
    public float gravityNormal = 3f;
    public float gravityMultNoUpKey = 2f;
    public float maxFallSpeed = 30f;
    public bool doNotReduceGravityGoingUp = true;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 2f;

    [Header("Status field")]
    public BarController hpBarController;
    public BarController manaController;

    private Rigidbody2D rb;

    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;


    public float Score { get; set; } = 0;

    private new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        Resistancies = new();

        hpBarController.SetValues(HP, true, 0, HP);
        manaController.SetValues(Mana, true, 0, Mana);
    }

    private new void Update()
    {
        base.Update();        
    }

    private new void FixedUpdate()
    {       
        HandleInput();
        base.FixedUpdate();
        manaController.SetValues(Mana, true);
        hpBarController.SetValues(HP, true);
    }

    private void HandleInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool left, right, up, shoot, melee;
        //Old trick: You want to get key inputs all at once so it isn't inconsistent.
        left = Input.GetKey(KeyCode.D);
        right = Input.GetKey(KeyCode.A);
        up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);

        if (up)
        {
            if (isGrounded)
                OnJump();
            if (rb.linearVelocityY > 0 && doNotReduceGravityGoingUp)
            {
                rb.gravityScale = gravityMultNoUpKey * gravityNormal;
            }
            else
            {
                rb.gravityScale = gravityNormal;
            }
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

    public override void OnDeath()
    {
        GameManager.Instance.EndPanel.Invoke("You died!", "Restart", () =>
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        });
    }
}
