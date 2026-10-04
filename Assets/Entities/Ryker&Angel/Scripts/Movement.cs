using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;

    [SerializeField] private float maxSpeed = 1f;
    [SerializeField] private float accelerationFrames = 5f;
    private float accelerationRate;
    [SerializeField] private float decelerationFrames = 3f;
    private float decelerationRate;
    private Vector2 direction;

    private TimeFlow timeFlow;

    // Ryker checks whether he's making a movement and based on that info he instantiates Angel on a position few frames earlier
    public Vector2 GetVelocity() => rb.velocity;
    public void StopMovement() {
        rb.velocity = Vector2.zero;
        movement = Vector2.zero;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        timeFlow = FindObjectOfType<TimeFlow>();

        direction = Vector2.zero;
    }

    void FixedUpdate()
    {
        Vector2 targetVelocity = movement * maxSpeed;
        accelerationRate = maxSpeed / accelerationFrames;
        decelerationRate = maxSpeed / decelerationFrames;

        // Log velocity and target velocity to better understand acceleration and deceleration
        // if (rb.velocity != Vector2.zero) Debug.Log(rb.velocity + " ; " + accelerationRate + " ; " + transform.position);

        if (movement.magnitude > 0) {
            rb.velocity = new Vector2(
                Mathf.MoveTowards(rb.velocity.x, targetVelocity.x, accelerationRate),
                Mathf.MoveTowards(rb.velocity.y, targetVelocity.y, accelerationRate)
            );
        } else {
            rb.velocity = new Vector2(
                Mathf.MoveTowards(rb.velocity.x, 0, decelerationRate),
                Mathf.MoveTowards(rb.velocity.y, 0, decelerationRate)
            );
        }

        if(timeFlow.getTimeFlow() == timeFlow.getSlowmoTime()){
            rb.velocity *= timeFlow.getTimeFlow();
        }

        // if (rb.velocity != Vector2.zero) Debug.Log(transform.position + " ; " + ((Vector2)transform.position + rb.velocity * Time.deltaTime));
    }

    public void Move(Vector2 value)
    {
        movement = value;
    }
}