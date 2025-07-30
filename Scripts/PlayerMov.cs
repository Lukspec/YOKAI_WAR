using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMov : MonoBehaviour
{    
    public SpriteRenderer sprite;
    public Rigidbody2D rb;
    public Animator anim;
    public TrailRenderer tr;
    public Transform wallCheck;
    public LayerMask wallLayer;
    
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public float damage = 2.7f;

    public GameObject HeavyAP;
    public float Hradius;
    public float Hdamage = 6.8f;

    private bool canMove;
    public float speed = 50f;
    public float sprint = 55f;
    private float horizontal;

    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 5f;

    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    public float jumpForce = 25f;
    public float sJump = 15f;
    public float airSpeed = 25f;
    public int pulosExtra = 1;

    public bool isGrounded;
    public Transform groundCheck;
    public LayerMask groundLayer;    

    public float KBForce;
    public float KBCount;
    public float KBTime;

    public bool isKnockRight;

    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
    }

    void Update()
    {
        KnockLogic();
        MoveLogic();
        Flip();
        WallSlide();
        AttackSys();

        if(isWalled() == true)
        {
            anim.SetBool("Sliding", true);
            anim.SetBool("Falling", false);
        }
        else if(isWalled() == false)
        {
            anim.SetBool("Sliding", false);
        }

        if (isDashing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            StartCoroutine(Dash());
        }

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded == true)
        {
            rb.velocity = new Vector2(0, jumpForce);
        }
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded == false && pulosExtra > 0)
        {
            rb.velocity = new Vector2(0, sJump);
            pulosExtra = 0;
        }
        if(isGrounded)
        {
            pulosExtra = 1;
        }

        if(isGrounded == true)
        {
            canMove = true;
        }
        else
        {
            canMove = false;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void AttackSys()
    {
        if(Input.GetMouseButtonDown(0))
        {
            anim.SetBool("Attack", true);
        }
        if(Input.GetMouseButtonDown(1))
        {
            anim.SetBool("heavyAttack", true);
        }
    }

    public void endAttack()
    {
        anim.SetBool("Attack", false);
    }
    public void endHeavyAttack()
    {
        anim.SetBool("heavyAttack", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        Gizmos.DrawWireSphere(HeavyAP.transform.position, Hradius);
    }

    private bool isWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }
    
    private void WallSlide()
    {
        if(isWalled() && !isGrounded && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    void KnockLogic()
    {
        if(KBCount < 0)
        {
            MoveLogic();
        }
        else
        {
            if(isKnockRight == true)
            {
                rb.velocity = new Vector2(-KBForce, KBForce);
            }
            if(isKnockRight == false)
            {
                rb.velocity = new Vector2(KBForce, KBForce);
            }
        }
        KBCount -= Time.deltaTime;
    }

    void MoveLogic()
    {
        if(canMove == true)
        {
            if(Input.GetKey(KeyCode.D))
            {
                horizontal = 1f;
                rb.AddForce(Vector2.right * speed);
                anim.SetBool("Walk", true);
                anim.SetBool("Idle", false);
            }
            else if(Input.GetKey(KeyCode.A))
            {
                horizontal = -1f;
                rb.AddForce(Vector2.right * -speed);
                anim.SetBool("Walk", true);
                anim.SetBool("Idle", false);
            }
            else
            {
                anim.SetBool("Walk", false);
                anim.SetBool("Idle", true);
            }

            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
            {
                rb.AddForce(Vector2.right * sprint);
                anim.SetBool("Run", true);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
            }
            else if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Vector2.right * -sprint);
                anim.SetBool("Run", true);
                anim.SetBool("Idle", false);
                anim.SetBool("Walk", false);
            }
            else
            {
                anim.SetBool("Run", false);
            }
            
            rb.drag = 10f;
            anim.SetBool("Falling", false);
        }
        else if(canMove == false)
        {
            if(Input.GetKey(KeyCode.D))
            {
                horizontal = 1f;
                rb.AddForce(Vector2.right * airSpeed);
            }
            else if(Input.GetKey(KeyCode.A))
            {
                horizontal = -1f;
                rb.AddForce(Vector2.right * -airSpeed);
            }
            
            rb.drag = 5f;

            anim.SetBool("Falling", true);
            anim.SetBool("Walk", false);
            anim.SetBool("Idle", false);
            anim.SetBool("Run", false);
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void Flip()
    {
        if(isFacingRight && horizontal < 0f || !isFacingRight && horizontal >0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
