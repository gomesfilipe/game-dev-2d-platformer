using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MOVEMENT_SPEED = 10.0f;
    public float JUMP_FORCE = 620.0f;

    // radio do circulo para determinar se o personagem está no chão
    float GROUND_CHECK_RADIUS = .2f;

    // os itens abaixo serao definidos no editor
    public LayerMask jumpableLayers;  // layers sobre os quais o personagem pode saltar
    public LayerMask enemyLayers;  // layers dos inimigos
    public LayerMask itemsLayers;  // layers dos itens
    public Transform groundCheckPosition;  // posicao do obj usado para ground check
    public Transform attackPoint;
    public Transform itemCollectPoint;
    public float attackRange = 0.5f;
    public float itemCollectRange = 0.5f;

    private Rigidbody2D rb;
    private Animator animator;

    private int collectedItems = 0;

    bool facingRight = true;

    public GameObject projectilePrefab;
    public GameObject laserPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
				return true;
		}

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        float vx = Input.GetAxisRaw("Horizontal") * MOVEMENT_SPEED;
        float vy = rb.velocity.y;  // not changed
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

        if (isGrounded && Input.GetButtonDown("Jump"))
            rb.AddForce(transform.up * JUMP_FORCE);

        if (Input.GetButtonDown("Fire1"))
            Attack();
            //Instantiate(projectilePrefab,
            //            gameObject.transform.position,
            //            gameObject.transform.rotation);

        if (Input.GetButtonDown("Fire2"))
        {
            Instantiate(laserPrefab,
                        gameObject.transform.position,
                        gameObject.transform.rotation);
        }

        // busca todos os itens com colisao abaixo do personagem
        Collider2D[] itemsColliders = Physics2D.OverlapCircleAll(
            itemCollectPoint.position,
            itemCollectRange,
            itemsLayers);

        collectedItems += itemsColliders.Length;

        foreach (Collider2D itemCollider in itemsColliders)
        {
            Destroy(itemCollider.gameObject);
        }

        Debug.Log("Collected items: " + collectedItems);
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
}



