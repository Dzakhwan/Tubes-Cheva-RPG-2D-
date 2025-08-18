    using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    private Vector2 CheckpointPos;
    private PlayerHealth playerHealth;
    public int facingDirection = 1;
    bool isRunning = false;
    private bool isKnockedBack = false;
    public Animator anim;

    private bool playingFootsteps = false;
    public float walkFootstepSpeed = 0.5f;
    public float runFootstepSpeed = 0.2f;

    private string currentFootstepSound = "Footstep";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckpointPos = transform.position;
        playerHealth = GetComponent<PlayerHealth>();
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
                if (!isRunning)
                {
                    isRunning = true;
                    if (playingFootsteps)
                    {
                        StopFootsteps();
                        StartFootsteps();
                    }
                }

                anim.SetBool("IsRunning", true);
                StatsManager.Instance.speed = 4.5f; // kecepatan berlari
            }
            else
            {
                if (isRunning)
                {
                    isRunning = false;
                    if (playingFootsteps)
                    {
                        StopFootsteps();
                        StartFootsteps();
                    }
                }
                anim.SetBool("IsRunning", false);
                StatsManager.Instance.speed = 3; // kecepatan jalan
            }
            rb.linearVelocity = new Vector2(horizontal, vertical) * StatsManager.Instance.speed;

            if ((horizontal != 0 || vertical != 0) && !playingFootsteps)
            {
                StartFootsteps();
            }
            else if (horizontal == 0 && vertical == 0 && playingFootsteps)
            {
                StopFootsteps();
            }
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

    void StartFootsteps()
    {
        CancelInvoke(nameof(PlayFootstep));

        playingFootsteps = true;
        float speed = isRunning ? runFootstepSpeed : walkFootstepSpeed;
        InvokeRepeating(nameof(PlayFootstep), 0f, speed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play(currentFootstepSound);
    }

    public void SetFootstepSound(string soundName)
    {
        currentFootstepSound = soundName;
    }
}
