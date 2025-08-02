    using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public Rigidbody2D rb;
    public int facingDirection = 1;
    bool isRunning = false;
    private bool isKnockedBack = false;
    public Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isKnockedBack == false)
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
                isRunning = true;
                anim.SetBool("IsRunning", true);

                StatsManager.Instance.speed = 6; // kecepatan berlari
            }
            else
            {
                anim.SetBool("IsRunning", false);
                StatsManager.Instance.speed = 3; // kecepatan jalan
            }
            rb.linearVelocity = new Vector2(horizontal , vertical ) * StatsManager.Instance.speed;
        }
    }
    void Flip()
    {
        facingDirection *= -1;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }
    public void knockback(Transform enemy, float force, float duration)
    {
        isKnockedBack = true;
        Vector2 direction = (transform.position - enemy.position).normalized;
        rb.linearVelocity = (direction * force);
        StartCoroutine(KnockbackCounter(duration));
    }
    IEnumerator KnockbackCounter(float duration)
    {
        yield return new WaitForSeconds(duration);
        rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }
}
