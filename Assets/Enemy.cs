using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 10f;

    public void TakeDamage(float val)
    {
        health -= val;

        if (health <= 0)
            Destroy(gameObject);
    }
}
