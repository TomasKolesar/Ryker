using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RykerShiftBound : MonoBehaviour
{
    private Movement movementScript;
    private Rigidbody2D rb;

    private bool isShifting = false;
    private Vector3 shiftDirection = new Vector3();
    public void SetIsShifting(bool value, Vector3 shiftDireciton) {
        shiftDirection = shiftDireciton;
        isShifting = value;
    }


    [Header("Bounding")]
    [SerializeField] private GameObject boundObject;
    private Collider2D coll;
    private bool isBounding = false;

    public void StartBoundingRykersPosition() {
        // Clamp position if supposed to be out of bounds
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, coll.bounds.min.x, coll.bounds.max.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, coll.bounds.min.y, coll.bounds.max.y);
        transform.position = clampedPosition;
        // protect before needing to clamp in future
        isBounding = true;
    }

    private void Start() {
        movementScript = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        coll = boundObject.GetComponent<Collider2D>();
    }

    void Update()
    {
        if(isBounding) {    
            // prevent from shifting, but only in direction off the collider
            if(isShifting) {
                bool isOutOfYLeft = shiftDirection.y < 0  && transform.position.y == coll.bounds.min.y;
                bool isOutOfYRight = shiftDirection.y > 0 && transform.position.y == coll.bounds.max.y;
                bool isOutOfXLeft = shiftDirection.x < 0 && transform.position.x == coll.bounds.min.x;
                bool isOutOfXRight = shiftDirection.x > 0 && transform.position.x == coll.bounds.max.x;

                if(isOutOfYLeft || isOutOfYRight || isOutOfXLeft || isOutOfXRight) {
                    rb.velocity = Vector2.zero;
                    // transform.position = positionBeforeShift;
                    isShifting = false;
                    movementScript.enabled = true;
                    return;
                }
                // Clamp position if supposed to be out of bounds
                Vector3 clampedPosition = transform.position;
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, coll.bounds.min.x, coll.bounds.max.x);
                clampedPosition.y = Mathf.Clamp(clampedPosition.y, coll.bounds.min.y, coll.bounds.max.y);
                Debug.Log("Clamped position: " + clampedPosition);
                transform.position = clampedPosition;
            }
        }
    }
}
