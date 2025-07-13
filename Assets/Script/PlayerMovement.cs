using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3;
    public Rigidbody2D rb;
    public int facingDirection = 1;
    bool isRunning = false;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        anim.SetFloat("horizontal", Mathf.Abs(horizontal));
        anim.SetFloat("vertical", Mathf.Abs(vertical));


        if (horizontal > 0 && transform.localScale.x < 0 || horizontal < 0 && transform.localScale.x > 0)
        {
            Flip();
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = 6; // kecepatan berlari
        }
        else
        {
            speed = 3; // kecepatan jalan
        }
        rb.linearVelocity = new Vector2(horizontal * speed, vertical * speed);
    }
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    void StaminaBar()
    {
        
    }
    
}
