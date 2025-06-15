using Assets.Core.Enums;

using Assets.Scripts;
using Assets.Scripts.Entities;

using Assets.Scripts.Objects.ScriptableObjects;

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : Entity
{
    [Header("Movement Settings")]
    public float accelPerSec = 80f;
    public float maxMoveSpeed = 20f;
    public float jumpSpeed = 20f;
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
    public TMP_Text scoreText;

    [Header("Audio Clips")]
    public AudioClip WoundedAudioClip;
    public AudioClip BulletBounce;
    public AudioClip RecievedDamage;

    private float moveInput;
    private bool isGrounded;
    private bool isRunning;
    private bool jumpPressed;

    private int reloadTime = 0;
    private float score = 0;
    private bool jumpedStillPressing = false;

    public float Score
    {
        get
        {
            return score;
        }
        set 
        {
            score = value;
            scoreText.text = $"Score: {score.ToString()}";
        }
    }

    private new void Start()
    {
        base.Start();
        Resistancies = new();

        hpBarController.SetValues(HP, true, 0, HP);
        manaController.SetValues(Mana, true, 0, Mana);

        if(GameManager.LastCheckpoint.y != float.NegativeInfinity)
        {
            transform.position = GameManager.LastCheckpoint;
            OwnedWeapons = GameManager.Player.OwnedWeapons;
            BindedWeapons = GameManager.Player.BindedWeapons;
            MeleeWeapons = GameManager.Player.MeleeWeapons;
            RangeWeapons = GameManager.Player.RangeWeapons;
            ProtectiveWeapons = GameManager.Player.ProtectiveWeapons;
            GameManager.Instance.SpellPanel.UpdatePanel();
        }
    }

    private new void Update()
    {
        if (!IsAlive || IsPaused)
            return;

        base.Update();
    }

    private new void FixedUpdate()
    {
        if(!IsAlive || IsPaused)
            return;

        Reload();
        HandleAudio();
        HandleInput();
        HandleAnimations();
        base.FixedUpdate();
        manaController.SetValues(Mana, true);
        hpBarController.SetValues(HP, true);
    }

    private void Reload()
    {
        if(reloadTime == 1)
        {
            GameManager.Instance.BulletInfoController.Reload();
            reloadTime = 0;
        }
        else if (reloadTime > 1)
        {
            reloadTime--;
        }
    }

    private void HandleAudio()
    {
        if (AudioSource == null)
            return;

        if (AudioSource.isPlaying)
            return;

        if (HP < MaxHP / 2 && HP > 0)
        {
            AudioSource.clip = WoundedAudioClip;
            AudioSource.Play();
        }
        else if(HP < 0)
        {
        }
        else
        {
            AudioSource.clip = null;
            AudioSource.Stop();
        }
    }

    private void HandleAnimations()
    {
        if (!isGrounded && !animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsJumping", true);
            animator.SetBool("IsRunning", false);
        }
        else if(isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        if(isRunning && !animator.GetBool("IsRunning") && !animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsRunning", true);
        }
        else if(!isRunning)
        {
            animator.SetBool("IsRunning", false);
        }
    }


    private void HandleInput()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius*Mathf.Min(transform.localScale.x, transform.localScale.y), groundLayer);//Messy fix for wall climbing: We adjusted the spherecast and need to adjust for scaling.

        bool left, right, up, shoot, melee;
        //Old trick: You want to get key inputs all at once so it isn't inconsistent.
        left = Input.GetKey(KeyCode.D);
        right = Input.GetKey(KeyCode.A);
        up = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);

        if (up)
        {
            if (isGrounded && !jumpedStillPressing)
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
            rb.gravityScale = gravityNormal * gravityMultNoUpKey;
            jumpedStillPressing = false;
        }


        bool activelyMoving = false;
        if (right && !left)
        {
            isRunning = true;
            IsMovingRight = false;
            OnMove(false);
            activelyMoving = true;
        }
        else if (left && !right)
        {
            isRunning = true;
            IsMovingRight = true;
            OnMove(true);
            activelyMoving = true;
        }
        else
        {
            isRunning = false;
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
                if (rb.linearVelocityX > accelPerSec * Time.fixedDeltaTime)
                {
                    rb.linearVelocityX -= accelPerSec * Time.fixedDeltaTime;
                }
                else rb.linearVelocityX = 0;
            }
            else if (rb.linearVelocityX < 0)
            {
                if (rb.linearVelocityX < -accelPerSec * Time.fixedDeltaTime)
                {
                    rb.linearVelocityX += accelPerSec * Time.fixedDeltaTime;
                }
                else rb.linearVelocityX = 0;
            }
        }
        if (rb.linearVelocityY < -maxFallSpeed)
        {
            rb.linearVelocityY = -maxFallSpeed;//Here we just set it since excess falling speeds contribute to hitbox mis-detections
        }
    }
    public void OnMove(bool toRight)
    {
        var dir = toRight ? 1 : -1;

        rb.linearVelocityX += dir * accelPerSec * Time.fixedDeltaTime;//We use fixed delta to be correct
    }

    public void OnJump()
    {
        rb.linearVelocityY = jumpSpeed;
        jumpedStillPressing = true;
    }

    public override void OnShoot(Weapon weapon, Vector2 dir)
    {
        if(weapon.Type == DamageType.Bullet)
        {
            if (!GameManager.Instance.BulletInfoController.HasBullet())
                return;

            GameManager.Instance.BulletInfoController.Shoot();

            if (!GameManager.Instance.BulletInfoController.HasBullet())
                reloadTime = 100;
        }
        base.OnShoot(weapon, dir);
    }

    public override void OnMelee(Weapon weapon, Vector2 dir)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, weapon.Range);
        Vector2 forward = IsMovingRight ? Vector2.right : Vector2.left;

        foreach (var hit in hits)
        {
            Vector3 objectDirection = ( hit.transform.position - transform.position ).normalized;
            float angleTo = Vector3.Angle(forward, objectDirection);

            if (angleTo <= 90f)
            {
                var controller = hit.GetComponent<ProjectileController>();
                if (controller != null)
                {
                    controller.Shooter = gameObject;
                    controller.rBody.linearVelocityX = 0;
                    controller.rBody.linearVelocityY = 0;
                    controller.Direction = objectDirection;
                    controller.Shoot();
                    AudioSource.PlayOneShot(BulletBounce);
                }
            }
        }

        base.OnMelee(weapon, dir);
    }

    public override void RecieveDamage(IDamager weapon)
    {
        AudioSource.PlayOneShot(RecievedDamage);
        base.RecieveDamage(weapon);
    }
    public override void OnDeath(bool destroy = false)
    {
        GameManager.Player = this;
        GameManager.PlayerDeathPosition = transform.position;
        GameManager.DroppedScore = Score;

        base.OnDeath(false);
        GameManager.Instance.EndPanel.Invoke("You died!", "Restart", () =>
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        });
    }
}
