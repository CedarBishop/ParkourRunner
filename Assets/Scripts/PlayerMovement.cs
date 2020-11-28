using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;
    public float jumpHeight;

    private Rigidbody2D rigidbody;

    private bool isGrounded;
    private bool isOnWall;
    private bool isSliding;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
#if UNITY_EDITOR
        KeyboardInput();
#else
        MobileInput();
#endif


    }

    void KeyboardInput ()
    {

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

        }
    }

    void WallJump ()
    {
        if (isSliding)
        {
            return;
        }
    }


}
