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
    private GameObject target;
    private float distance; // Distance between enemy and player
    private float intTimer;
    private float directionChangeTimer = 0f; // The time at which the last direction change has occurred at patrol
    private float directionChangeTheshold = 0.5f; // The threshold at which the direction change occurs at patrol
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
            target = GameObject.Find("Player");
            inRange = true;
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
            MoveBetweenPointAAndPointB();
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

    void MoveBetweenPointAAndPointB()
    {
        directionChangeTimer -= Time.deltaTime;

        if (Mathf.Abs(transform.position.x - targetPositionPatrol.x) < 0.01 && directionChangeTimer < directionChangeTheshold)
        {
            Debug.Log("Trocou de direcao " + directionChangeTimer);

            directionChangeTimer = directionChangeTheshold + 1f;
            //targetPositionPatrol = targetPositionPatrol == new Vector2(pointA.position.x, pointA.position.y) ? pointB.position : pointA.position;
            targetPositionPatrol = Vector2.Distance(transform.position, pointA.position) < Vector2.Distance(transform.position, pointB.position) ? pointB.position : pointA.position;
        }

        float desirable_speed;

        // Se move pra esquerda
        if (transform.position.x > targetPositionPatrol.x)
        {
            desirable_speed = -moveSpeed;
            if (facingRight)
            {
                Flip();
            }
        }
        // Se move pra direita
        else
        {
            desirable_speed = moveSpeed;
            if (!facingRight)
            {
                Flip();
            }
        }

        rb.velocity = new Vector2(desirable_speed, rb.velocity.y);
    }

    void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target.transform.position);

        if (distance > attackDistance)
        {
            Move();
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
        //animator.SetBool("canWalk", true);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(enemyAttackAnimationName))
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, transform.position.y);

            rb.velocity = new Vector2((targetPosition.x - transform.position.x) > 0 ? moveSpeed : -moveSpeed, rb.velocity.y);

            //transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            //Vector2 newPosition = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
            //rb.MovePosition(newPosition);
        }

        if (transform.position.x > target.transform.position.x && facingRight)
        {
            Flip();
        }
        else if (transform.position.x < target.transform.position.x && !facingRight)
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
}
