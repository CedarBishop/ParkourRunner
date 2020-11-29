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

    public GameObject slideGFX;
    public GameObject standGFX;

    private Rigidbody2D rigidbody;

    public bool facingRight;
    private bool isGrounded;
    private bool isOnWall;
    private bool isSliding;

    private float speedMultiplier = 1;

    private Vector3 fp;   //First touch position
    private Vector3 lp;   //Last touch position
    private float dragDistance;  //minimum distance for a swipe to be registered

    void Start()
    {
        dragDistance = Screen.height * 15 / 100; //dragDistance is 15% height of the screen
        rigidbody = GetComponent<Rigidbody2D>();
        facingRight = true;
        speedMultiplier = 1;
        slideGFX.SetActive(false);
        standGFX.SetActive(true);
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

        rigidbody.velocity = new Vector2(((facingRight)?movementSpeed : -movementSpeed) * Time.deltaTime * speedMultiplier, rigidbody.velocity.y);
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
        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                fp = touch.position;
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                lp = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                lp = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(lp.x - fp.x) > dragDistance || Mathf.Abs(lp.y - fp.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lp.x - fp.x) > Mathf.Abs(lp.y - fp.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((lp.x > fp.x))  //If the movement was to the right)
                        {   //Right swipe
                            Debug.Log("Right Swipe");
                            WallJump();
                        }
                        else
                        {   //Left swipe
                            Debug.Log("Left Swipe");
                            WallJump();
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (lp.y > fp.y)  //If the movement was up
                        {   //Up swipe
                            Debug.Log("Up Swipe");
                            Jump();
                        }
                        else
                        {   //Down swipe
                            Debug.Log("Down Swipe");
                            Slide();
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");
                }
            }
        }
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
        slideGFX.SetActive(true);
        standGFX.SetActive(false);
        speedMultiplier = 2f;
        yield return new WaitForSeconds(slideDuration);
        isSliding = false;
        speedMultiplier = 1;
        slideGFX.SetActive(false);
        standGFX.SetActive(true);
        Debug.Log("PlayerMovement::CoSlide: End Slide");
    }

    void WallJump ()
    {
        if (isSliding)
        {
            return;
        }
        if (isGrounded)
        {
            return;
        }

        if (isOnWall)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2 * (Physics2D.gravity.y * rigidbody.gravityScale));
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpVelocity);

            facingRight = !facingRight;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(groundCheckOffset.x, groundCheckOffset.y, 0), new Vector3(groundCheckSize.x, groundCheckSize.y));
        Gizmos.DrawWireCube(transform.position + new Vector3((facingRight)?wallCheckOffset.x: -wallCheckOffset.x, wallCheckOffset.y, 0), new Vector3(wallCheckSize.x, wallCheckSize.y));
    }

}
