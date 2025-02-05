using System.Collections;
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
    public float bufferTimeLimit = 0;

    private enum WalkDirection { Right = 1, Left = -1 };
    private WalkDirection walkDirection = WalkDirection.Right;
    private float timeLimit = 0.2f;
    private float timeElapsed;
    private float valueToKeep;
    private float bufferTime = 0;
    private float bufferMoveInput;


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
        timeElapsed = timeLimit;
    }

    private void FixedUpdate()
    {
        manageWallMovement();
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocityY);

    }

    private void manageWallMovement()
    {
        if (bufferTime > 0)
        {
            bufferTime -= Time.fixedDeltaTime;
            if (!terrainInteractions.IsOnWall)
            {
                moveInput.x = bufferMoveInput;
            }
        }
        if (terrainInteractions.IsOnWall)
        {
            rb.linearVelocityY = -slidingSpeed;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (!terrainInteractions.IsOnWall)
        {
            IsRunning = moveInput.x != 0;
            if (moveInput.x != 0 && moveInput.x != (float)walkDirection)
            {
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
                walkDirection = (WalkDirection)moveInput.x;
            }
        }
        else
        {
            if( moveInput.x == -(float)walkDirection)
            {
                terrainInteractions.IsOnWall = false;
            }
            else if(moveInput.x != 0)
            {
                bufferTime = bufferTimeLimit;
                bufferMoveInput = moveInput.x;
            }
            else
            {
                moveInput.x = 0;
            }
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
                moveInput.x = transform.localScale.x > 0 ? -1 : 1;
                StartCoroutine(JumpCoroutine(timeLimit, valueToKeep));
            }
            rb.linearVelocityY = jumpStrength;
            terrainInteractions.CanJump = false;
        }

    }

    private IEnumerator JumpCoroutine(float timeLimit, float value)
    {
        float elapsed = 0f;

        while (elapsed < timeLimit)
        {
           
            rb.linearVelocityY = jumpStrength/2;
            elapsed += Time.fixedDeltaTime;
            if (terrainInteractions.IsGrounded || terrainInteractions.IsOnWall)
                StopCoroutine("JumpCoroutine");
            yield return new WaitForFixedUpdate();
        }
        moveInput.x = 0;
    }
}
