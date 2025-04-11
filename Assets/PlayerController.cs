using System;
using Unity.XR.OpenVR;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem; // New Input System

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 30f;
    public float moveSpeedCap = 100f;
    public float jumpForce = 300f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.4f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private bool jumpPressed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
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

        if (Input.GetKey(KeyCode.A))
        {
            OnMove(false);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            OnMove(true);
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
}
