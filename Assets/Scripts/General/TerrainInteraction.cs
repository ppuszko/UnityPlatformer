using UnityEngine;

public class TerrainInteraction : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public BoxCollider2D boxCol;
    public float groundToleration = 0.05f;
    public float wallToleration = .1f;

    private CapsuleCollider2D col;
    
    private Animator animator;
    private RaycastHit2D[] groundHits = new RaycastHit2D[5];
    private RaycastHit2D[] wallHits = new RaycastHit2D[5];
    private RaycastHit2D[] coyoteHits = new RaycastHit2D[5];
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    private bool _isGrounded;
    public bool IsGrounded
    {
        get { return _isGrounded; }
        set {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    private bool _canJump;
    public bool CanJump
    {
        get { return _canJump; }
        set { _canJump = value;
            if (IsOnWall)
                _canJump = true;
            animator.SetBool(AnimationStrings.canJump, _canJump);
        }
    }

    private bool _isOnWall;
    public bool IsOnWall
    {
        get { return _isOnWall; }
        set {
            _isOnWall = value;
            if (IsGrounded)
                _isOnWall = false;
            animator.SetBool(AnimationStrings.isOnWall, _isOnWall);
        }
    }

    private void Awake()
    {
        col = GetComponent<CapsuleCollider2D>();
        //boxCol = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsGrounded = col.Cast(Vector2.down, castFilter, groundHits, groundToleration) > 0;
        IsOnWall = col.Cast(wallCheckDirection, castFilter, wallHits, wallToleration) > 0;
        CanJump = boxCol.Cast(Vector2.down, castFilter, coyoteHits, groundToleration) > 0;
    }
}
