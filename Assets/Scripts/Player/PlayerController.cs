using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveInput;
    private TerrainInteraction terrainInteractions;

    public float jumpStrength = 5f;
    public float gravityFactor = 10f;
    public float moveSpeed = 5f;
    public float slidingSpeed = 3;

    private enum WalkDirection {Right = 1, Left = -1};
    private WalkDirection walkDirection = WalkDirection.Right;

    private bool _isRunning = false;
    public bool IsRunning
    {
        get { return _isRunning; }
        set {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityFactor;
        terrainInteractions = GetComponent<TerrainInteraction>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);
        if (terrainInteractions.IsOnWall && terrainInteractions.CanJump)
        {
            rb.linearVelocity = new Vector2(0, -slidingSpeed);
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!terrainInteractions.IsOnWall)
        {
            moveInput = context.ReadValue<Vector2>();
            IsRunning = moveInput.x != 0;
            if (moveInput.x != 0 && moveInput.x != (float)walkDirection)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                walkDirection = (WalkDirection)moveInput.x;
            }
        }
        else
        {
            //rb.linearVelocity = new Vector2(0, -slidingSpeed);
            moveInput = Vector2.zero;
            IsRunning = false;
        }
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && terrainInteractions.CanJump)
        {
            if (terrainInteractions.IsOnWall)
            {
                terrainInteractions.IsOnWall = false;
                moveInput = new Vector2(moveSpeed * -transform.localScale.x, jumpStrength);
                Debug.Log("dupa");
            
            }
            else
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
            }
            terrainInteractions.CanJump = false;
        }
        
    }
}
