using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Animator animator;

    public float health = 10f;

    public void TakeDamage(float val)
    {
        health -= val;

        animator.SetTrigger("hit");

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetBool("isDead", true);

        GetComponent<Collider2D>().enabled = false;

        this.enabled = false;
    }
}
