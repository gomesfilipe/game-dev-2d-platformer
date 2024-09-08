using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform leftPlatform;
    public Transform middlePlatform;
    public Transform rightPlatform;
    public Transform startPoint;
    public Transform endPoint;

    public int direction = 1;
    public float speed = 1.5f;

    List<Transform> platforms;

    void Start() {
        platforms = new List<Transform> {
            leftPlatform,
            middlePlatform,
            rightPlatform
        };
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlatformPositions();

        if (ShouldChangeDirection()) {
            direction *= -1;
        }
    }

    void UpdatePlatformPositions()
    {
        Vector2 target = currentmovementTarget();
        
        middlePlatform.position = Vector2.MoveTowards(middlePlatform.position, target, speed * Time.deltaTime);
        leftPlatform.position = new Vector2(middlePlatform.position.x - 1, middlePlatform.position.y);
        rightPlatform.position = new Vector2(middlePlatform.position.x + 1, middlePlatform.position.y);
    }

    bool ShouldChangeDirection()
    {
        Vector2 target = currentmovementTarget();
        
        foreach (Transform platform in platforms) {
            float distance = (target - (Vector2) platform.position).magnitude;

            if (distance <= 0.1f) {
                return true;
            }
        }

        return false;
    }

    Vector2 currentmovementTarget()
    {
        return direction == 1
            ? startPoint.position
            : endPoint.position;
    }
}
