using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpHeight = 2f; // Jump height
    public float gravityScale = 3f; // Gravity scale (multiplies with gravity)
    public float doubleJumpHeight = 1.5f; // Double jump height

    private Rigidbody2D rb; // Reference to the Rigidbody2D
    private bool isGrounded; // To check if the player is on the ground
    private bool canDoubleJump; // To check if double jump is available

    private Animator animator; // Reference to the Animator

    [SerializeField] private float groundCheckRadius = 0.2f; // Radius for checking ground collision
    public Transform groundCheck; // The position to check if the player is grounded
    public Transform startPosition;

    [Header("Animation Control")]
    [SerializeField] private float minAnimSpeed = 1f;
    [SerializeField] private float maxAnimSpeed = 2f;
    [SerializeField] private float worldScrollSpeed = 5f; // Reference to your current scroll speed
    [SerializeField] private float scrollSpeedAtMinAnimSpeed = 5f; // At this speed, anim speed will be minAnimSpeed
    [SerializeField] private float scrollSpeedAtMaxAnimSpeed = 30f; // At this speed, anim speed will be maxAnimSpeed

    void Start()
    {
        startPosition = this.transform;
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        animator = GetComponent<Animator>(); // Get the Animator component
        animator.speed = 1f;
        EventManager.Instance.OnGameRestart += ResetPosition;
        EventManager.Instance.OnWorldSpeedUpdate += UpdateWorldScrollSpeed;
    }

    private void FixedUpdate()
    {
        // Check if player is grounded (use a small circle at the player's feet)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"));
        animator.SetBool("IsGrounded", isGrounded);
    }

    void Update()
    {
        // Reset double jump ability if grounded
        if (isGrounded)
        {
            canDoubleJump = true;
            animator.SetBool("IsJumping", false); // Stop jump animation when grounded
            animator.SetBool("IsFalling", false); // Stop falling animation when grounded

            float t = Mathf.InverseLerp(scrollSpeedAtMinAnimSpeed, scrollSpeedAtMaxAnimSpeed, worldScrollSpeed);
            float animSpeed = Mathf.Lerp(minAnimSpeed, maxAnimSpeed, t);
            animator.speed = animSpeed;
        }
        else
        {
            // During jump or fall, use default animation speed
            animator.speed = 1f;
        }

        // Handle Jump (single or double)
        if (Input.GetButtonDown("Jump") || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (isGrounded)
            {
                // Apply the single jump force
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(jumpHeight * -2f * Physics2D.gravity.y * gravityScale));
                animator.SetBool("IsJumping", true); // Play jump animation
                EventManager.Instance.TriggerOnPlayerJumped();
            }
            else if (canDoubleJump)
            {
                // Apply the double jump force
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sqrt(doubleJumpHeight * -2f * Physics2D.gravity.y * gravityScale));
                canDoubleJump = false; // Disable double jump after use
                animator.SetBool("IsJumping", true); // Play jump animation
                EventManager.Instance.TriggerOnPlayerDoubleJumped();
            }
        }

        // Apply falling animation if not grounded
        if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetBool("IsFalling", true); // Play falling animation
            animator.SetBool("IsJumping", false);
        }
        else if (rb.velocity.y > 0)
        {
            animator.SetBool("IsFalling", false); // Stop falling animation if jumping
        }
    }

    private void ResetPosition()
    {
        Vector3 newPos = startPosition.position;
        transform.position = newPos;

        animator.speed = 1f;
    }

    private void UpdateWorldScrollSpeed(float speed)
    {
        worldScrollSpeed = speed;
    }


    // Visualize ground check radius in the editor (optional)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
