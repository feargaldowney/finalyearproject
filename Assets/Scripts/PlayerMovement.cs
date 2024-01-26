using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite; 
    private Animator anim;
    // [SerializeField]private Joystick joystick;
    // [SerializeField]private JumpButton jumpButton;


    [SerializeField] private LayerMask jumpableGround;
    // Serialize Field allows these values to be changed within unity under the component
    // Note, they would also be exposed to the editor if they were public, but ofc they would then be exposed to the whole application
    // If I were to change these values below, I would also have to right click on the script in unity and click reset values.
    [SerializeField]private float dirX = 0f;
    [SerializeField]private float moveSpeed = 7f;
    [SerializeField]private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling}

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal"); // Horizontal matches input manager
        // dirX = joystick.Horizontal; // Horizontal matches input manager

        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);


        // Input.GetKey for holding down 
        // Input.GetKeyDown works for press but button works under project settings in unity. Makes use of unitys input manager.
        if (Input.GetButtonDown("Jump") && IsGrounded()) // name of the input is important, must match unity input manager
            {
                // Vector2 is a vector for 2 values, x, y & z
                // Instead of 0 for x, we set it to the previous frames velocity to make it smoother
                GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX >0f)
        {
            state = MovementState.running;            
            sprite.flipX = false;
        }
        else if (dirX < 0)
        {
            state = MovementState.running;            
            sprite.flipX = true;
        }
        else 
        {
            state = MovementState.idle;            
        }

        // Overwriting state upon jump, as jump has priority
        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);
    }

    // public void Jump(){
    //     GetComponent<Rigidbody2D>().velocity = new Vector2(rb.velocity.x, jumpForce);
    // }
    private bool IsGrounded()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }

    // public bool speak()
    // {
    //     if 
    // }

// TODO:
// Fix jumping so that player can only jump once
// Include microphone asset.

}
