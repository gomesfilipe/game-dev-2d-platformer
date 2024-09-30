using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public BoxCollider2D triggerArea;
    public Animator animator;
    public Transform groundCheckPosition;
    public Transform rayCast;
    public Transform pointA;
    public Transform pointB;
    public LayerMask raycastMask;
    public LayerMask jumpableLayers;  // layers sobre os quais o personagem pode saltar
    public float rayCastLength;
    public float attackDistance; // Minimum distance for attack
    public float moveSpeed;
    public float timer; //Timer for cooldown between attacks

    public string enemyAttackAnimationName;

    public float health = 10f;

    public int deathDelay = 3;

    float GROUND_CHECK_RADIUS = .1f;
    bool facingRight = true;

    private RaycastHit2D hit;
    private Transform target;
    private float distance; // Distance between enemy and player
    private float intTimer;
    private bool attackMode;
    private bool inRange; // Check if Player is in range
    private bool cooling;
    private Rigidbody2D rb;
    private Vector2 targetPositionPatrol;

    bool isGrounded()
    {
        // busca todos os itens com colisao abaixo do personagem
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            groundCheckPosition.position,
            GROUND_CHECK_RADIUS,
            jumpableLayers);

        // se pelo menos uma colisao nao eh com o proprio
        // personsagem, o personagem esta' no chao.

        return colliders.Length > 0;
    }

    void Awake()
    {
        SelectTarget();
        intTimer = timer;   
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Pega o ponto mais longe
        targetPositionPatrol = Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointB.position : pointA.position;
    }

    void FixedUpdate()
    {
        bool isTouchingPlayer = triggerArea.IsTouchingLayers(LayerMask.GetMask("Player"));

        if (isTouchingPlayer)
        {
            target = GameObject.Find("Player").transform;
            inRange = true;
            //Flip();
        }

        if (!attackMode)
        {
            Move();
        }

        if (!InsideOfLimits() && !inRange && !animator.GetCurrentAnimatorStateInfo(0).IsName(enemyAttackAnimationName))
        {
            SelectTarget();
        }

        if (inRange)
        {
            //hit = Physics2D.Raycast(rayCast.position, Vector2.right, rayCastLength, raycastMask);
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        if (hit.collider != null)
        {
            EnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (!inRange)
        {
            //rb.velocity = new Vector2(0, rb.velocity.y);
            //MoveBetweenPointAAndPointB();
            StopAttack();
        }

        //if (Mathf.Abs(rb.velocity.x) > 0.0001)
        //{
        //    animator.SetFloat("speed", moveSpeed);
        //}
        //else
        //{
        //    animator.SetFloat("speed", 0);
        //}

        //animator.SetFloat("speed", Mathf.Abs(rb.velocity.x * moveSpeed / Time.deltaTime));
        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        animator.SetBool("isJumping", !isGrounded()); 
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            //animator.SetBool("Attack", false);
        }
    }

    void Move()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(enemyAttackAnimationName))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            rb.velocity = new Vector2((targetPosition.x - transform.position.x) > 0 ? moveSpeed : -moveSpeed, rb.velocity.y);
        }

        if (transform.position.x > target.position.x && facingRight)
        {
            Flip();
        }
        else if (transform.position.x < target.position.x && !facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void Attack()
    {
        timer = intTimer; // Reset Timer when Fireball is cast
        attackMode = true; // To check if the Enemy can still attack or not
        cooling = true; // Start the Timer to Cooldown

        //animator.SetBool("canWalk", false);
        animator.SetTrigger("Attack");
    }

    void StopAttack()
    {
        cooling = false;
        attackMode = false;
        animator.ResetTrigger("Attack");
    }

    void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    public void TakeDamage(float val)
    {
        health -= val;

        animator.SetTrigger("hit");

        if (health == 0)
        {
            Die();
        }
        else
        {
            animator.SetBool("isDead", false);
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        //animator.SetTrigger("deathAnim");
        //Invoke("EraseEnemy", deathDelay);
        EraseEnemy();
    }

    void EraseEnemy()
    {
        Debug.Log("Enemy is dead");
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        //GetComponent<Rigidbody2D>().simulated = false;
    }

    void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
        }
    }

    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfLimits()
    {
        Transform rightPoint = pointA.position.x > pointB.position.x ? pointA : pointB;
        Transform leftPoint = pointA.position.x > pointB.position.x ? pointB : pointA;

        return transform.position.x > leftPoint.position.x && transform.position.x < rightPoint.position.x;
    }

    private void SelectTarget()
    {
        Transform rightPoint = pointA.position.x > pointB.position.x ? pointA : pointB;
        Transform leftPoint = pointA.position.x > pointB.position.x ? pointB : pointA;

        float distanceToLeft = Vector2.Distance(transform.position, leftPoint.position);
        float distanceToRight = Vector2.Distance(transform.position, rightPoint.position);

        if (distanceToLeft > distanceToRight)
        {
            target = leftPoint;
        }
        else
        {
            target = rightPoint;
        }

        Flip();
    }
}
