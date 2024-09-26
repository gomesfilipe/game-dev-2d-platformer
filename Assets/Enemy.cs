using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;
    public Transform groundCheckPosition;
    public LayerMask jumpableLayers;  // layers sobre os quais o personagem pode saltar

    public float health = 10f;

    public int deathDelay = 3;

    float GROUND_CHECK_RADIUS = .1f;
    bool facingRight = true;

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

    private void Update()
    {
        //animator.SetFloat("speed", Mathf.Abs(aiPath.desiredVelocity.x));

        animator.SetBool("isJumping", !isGrounded()); 
    }

    public void TakeDamage(float val)
    {
        health -= val;

        animator.ResetTrigger("hit");
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
}
