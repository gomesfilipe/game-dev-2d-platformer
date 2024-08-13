using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LayerMask jumpableLayers;
    LineRenderer lineRenderer;
    float lifeSpanSeconds = 2f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        RaycastHit2D hitInfo = Physics2D.Raycast(gameObject.transform.position,
                                        gameObject.transform.right,
                                        Mathf.Infinity,
                                        jumpableLayers);

        if (hitInfo)
        {
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            if (enemy)
                enemy.TakeDamage(1f);

            lineRenderer.SetPosition(0, gameObject.transform.position);
            lineRenderer.SetPosition(1, hitInfo.point);
        }
    }

    void Update()
    {
        lifeSpanSeconds -= Time.deltaTime;

        if (lifeSpanSeconds <= 0)
            Destroy(gameObject);
    }
}
