using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public float health = 10f;

    public int deathDelay = 3;

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
