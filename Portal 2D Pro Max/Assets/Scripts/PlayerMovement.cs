using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;

    public float moveSpeed;
    public float jumpForce;
    private bool isJumping;
    private float moveHorizontal;
    private float moveVertical;
    public float maxSpeed;
    private bool didJump;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        didJump = false;
        isJumping = false;
    }
    
    void FixedUpdate()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        moveVertical = Input.GetAxisRaw("Vertical");

        rigidbody2D.velocity = Vector3.ClampMagnitude(rigidbody2D.velocity, maxSpeed);
        if (moveHorizontal > 0.1f || moveHorizontal < -0.1f)
        {
            rigidbody2D.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse);
        }

        if (!isJumping && moveVertical > 0.1f && !didJump)
        {
            rigidbody2D.AddForce(new Vector2(0f, moveVertical * jumpForce), ForceMode2D.Impulse);

            didJump = true;
            
            StartCoroutine(passiveMe(0.35f));
 
            IEnumerator passiveMe(float secs)
            {
                yield return new WaitForSecondsRealtime(secs);
                didJump = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }
}