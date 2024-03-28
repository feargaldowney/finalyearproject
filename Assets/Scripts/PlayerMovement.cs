using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private LayerMask jumpableGround;
    [SerializeField] private AudioSource jumpSoundEffect;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;
    private Animator anim;

    private bool canJump = true; // Flag to control jumping ability

    public enum MovementState { idle, running, jumping, falling }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float dirX = joystick.Horizontal;
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformJump();
        }

        UpdateAnimationState(dirX);
    }

    public void OnJumpButton()
    {
        PerformJump();
    }

    private void PerformJump()
    {
        if (ItemCollector.isPaused)
        {
            UnpauseGame();
        }

        if (canJump)
        {
            Jump();
        }
    }

    public void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        jumpSoundEffect.Play();
        canJump = false; // Disable jumping until grounded again
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            canJump = true; // Allow jumping again once grounded
        }
    }

    private void UpdateAnimationState(float dirX)
    {
        MovementState state = dirX > 0f ? MovementState.running : dirX < 0 ? MovementState.running : MovementState.idle;
        sprite.flipX = dirX < 0;
        if (rb.velocity.y > .1f) state = MovementState.jumping;
        else if (rb.velocity.y < -.1f) state = MovementState.falling;
        anim.SetInteger("state", (int)state);
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1f;
        ItemCollector.isPaused = false;
    }
}
