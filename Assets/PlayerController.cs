using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float BASE_MOVEMENT_SPEED = 10.0f;

    float MOVEMENT_SPEED = 10.0f;
    public float JUMP_FORCE = 620.0f;

    public float SUPER_JUMP_POWER_FORCE = 100.0f;
    public float PROPULSION_FORCE = 3.0f;

    // radio do circulo para determinar se o personagem está no chão
    float GROUND_CHECK_RADIUS = .3f;

    // os itens abaixo serao definidos no editor
    public LayerMask jumpableLayers;  // layers sobre os quais o personagem pode saltar
    public LayerMask enemyLayers;  // layers dos inimigos
    public LayerMask itemsLayers;  // layers dos itens

    public LayerMask jumpSuperPowerLayers;
    public LayerMask propulsionLayers;
    public Transform groundCheckPosition;  // posicao do obj usado para ground check
    public Transform attackPoint;
    public Transform itemCollectPoint;
    public float attackRange = 0.5f;
    public float itemCollectRange = 0.5f;

    public float propulsionDelay = 1.0f;

    private Rigidbody2D rb;
    private Animator animator;

    private int collectedItems = 0;
    private int itemsToCollect = 9999;

    bool facingRight = true;

    public GameObject projectilePrefab;
    public GameObject laserPrefab;

    private CanvasController canvasControllerPlayer;

    private float lastTimePropulsion;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar; 

    void Start()
    {
        MOVEMENT_SPEED = BASE_MOVEMENT_SPEED;

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        canvasControllerPlayer = CanvasController.canvasController;
        Debug.Log(canvasControllerPlayer);
        canvasControllerPlayer.score = 0;


        Collider2D[] itemsColliders = Physics2D.OverlapCircleAll(
           itemCollectPoint.position,
           int.MaxValue,
           itemsLayers);

        itemsToCollect = itemsColliders.Length;

        canvasControllerPlayer.scoreText.text = canvasControllerPlayer.score.ToString() + " / " + itemsToCollect.ToString();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        lastTimePropulsion = Time.time;
    }

    bool IsGrounded()
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

    // Update is called once per frame
    void Update()
    {
        float vx = Input.GetAxisRaw("Horizontal") * MOVEMENT_SPEED;
        
        float vy = (rb.IsTouchingLayers(propulsionLayers) && (collectedItems >= itemsToCollect))
            ? PROPULSION_FORCE
            : rb.velocity.y;

        rb.velocity = new Vector2(vx, vy);

        animator.SetFloat("speed", Mathf.Abs(vx));

        if ((vx > 0 && !facingRight) || (vx < 0 && facingRight))
        {
            facingRight = !facingRight;
            transform.Rotate(0f, 180f, 0f);
        }

        bool isGrounded = IsGrounded();

        animator.SetBool("isJumping", !isGrounded);
        // animator.SetBool("isJumping", false);

        if (isGrounded)
            MOVEMENT_SPEED = BASE_MOVEMENT_SPEED;

        if (isGrounded && Input.GetButtonDown("Jump"))
            rb.AddForce(transform.up * JUMP_FORCE);

        if (Input.GetButtonDown("Fire1"))
            Attack();
            //Instantiate(projectilePrefab,
            //            gameObject.transform.position,
            //            gameObject.transform.rotation);

        //if (Input.GetButtonDown("Fire2"))
        //{
        //    Instantiate(laserPrefab,
        //                gameObject.transform.position,
        //                gameObject.transform.rotation);
        //}

        // busca todos os itens com colisao abaixo do personagem
        Collider2D[] itemsColliders = Physics2D.OverlapCircleAll(
            itemCollectPoint.position,
            itemCollectRange,
            itemsLayers);

        collectedItems += itemsColliders.Length;

        foreach (Collider2D itemCollider in itemsColliders)
        {
            canvasControllerPlayer.score++;
            canvasControllerPlayer.scoreText.text = canvasControllerPlayer.score.ToString() + " / " + itemsToCollect.ToString();

            Destroy(itemCollider.gameObject);
        }

        // Debug.Log("Collected items: " + collectedItems);
        SuperJumpPower();
    }

    void SuperJumpPower()
    {
        Collider2D[] superJumpPowerColliders = Physics2D.OverlapCircleAll(
            itemCollectPoint.position,
            itemCollectRange,
            jumpSuperPowerLayers);

        foreach (Collider2D superJumpPowerCollider in superJumpPowerColliders)
        {            
            rb.AddForce(transform.up * SUPER_JUMP_POWER_FORCE);
            MOVEMENT_SPEED = 20.0f;
        }

    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(1);
        }
    }

    public void TakeDamege(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
}



