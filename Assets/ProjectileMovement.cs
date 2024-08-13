using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float projectileSpeed = 20f;
    public float projectileDamage = 1f;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity =
            transform.right * projectileSpeed;
    }

    void Update()
    {
        if (Mathf.Abs(gameObject.transform.position.x) > 50)
            Destroy(gameObject);
    }

    // chamado quando o collider2d detecta uma colisao
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        if (hitInfo.name != "Player")
            Destroy(gameObject);

        Enemy enemy_script = hitInfo.GetComponent<Enemy>();

        if (enemy_script)
            enemy_script.TakeDamage(projectileDamage);
    }
}
