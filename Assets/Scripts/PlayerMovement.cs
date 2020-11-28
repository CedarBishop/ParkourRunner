using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float jumpHeight;
    public float slideDuration;

    public Vector2 groundCheckOffset;
    public Vector2 groundCheckSize;
    public LayerMask groundLayer;

    public Vector2 wallCheckOffset;
    public Vector2 wallCheckSize;
    public LayerMask wallLayer;

    public Collider2D topCollider;

    private Rigidbody2D rigidbody;

    private bool isGrounded;
    private bool isOnWall;
    private bool isSliding;
    public bool facingRight;

    private float speedMultiplier = 1;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        facingRight = true;
        speedMultiplier = 1;
    }

    void Update()
    {
#if UNITY_EDITOR
        KeyboardInput();
#else
        MobileInput();
#endif
        isGrounded = Physics2D.OverlapBox(groundCheckOffset + new Vector2(transform.position.x, transform.position.y), groundCheckSize, 0, groundLayer);
        isOnWall = Physics2D.OverlapBox(new Vector2((facingRight) ? wallCheckOffset.x : -wallCheckOffset.x, wallCheckOffset.y) + new Vector2(transform.position.x, transform.position.y), wallCheckSize, 0, wallLayer);

        rigidbody.velocity = new Vector2((facingRight)?movementSpeed : -movementSpeed * Time.deltaTime * speedMultiplier, rigidbody.velocity.y);
    }

    void KeyboardInput ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
    }

    void MobileInput ()
    {

    }

    void Jump ()
    {
        if (isSliding)
        {
            return;
        }
        if (isOnWall)
        {
            WallJump();
        }
        else if (isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigidbody.gravityScale));
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpVelocity);
        }
    }

    void Slide ()
    {
        if (isSliding)
        {
            return;
        }
        if (isGrounded)
        {
            StartCoroutine("CoSlide");
        }
    }

    IEnumerator CoSlide ()
    {
        Debug.Log("PlayerMovement::CoSlide: Start Slide");
        isSliding = true;
        topCollider.enabled = false;
        speedMultiplier = 1.5f;
        yield return new WaitForSeconds(slideDuration);
        isSliding = false;
        topCollider.enabled = true;
        speedMultiplier = 1;
        Debug.Log("PlayerMovement::CoSlide: End Slide");
    }

    void WallJump ()
    {
        if (isSliding)
        {
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(groundCheckOffset.x, groundCheckOffset.y, 0), new Vector3(groundCheckSize.x, groundCheckSize.y));
        Gizmos.DrawWireCube(transform.position + new Vector3((facingRight)?wallCheckOffset.x: -wallCheckOffset.x, wallCheckOffset.y, 0), new Vector3(wallCheckSize.x, wallCheckSize.y));
    }

}
