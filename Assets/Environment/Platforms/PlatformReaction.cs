using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformReaction : MonoBehaviour
{
    // Boolean that determines whether FixedUpdate() executes or not
    private bool canBoost;
    
    // Attributes of boost that need to be obtained from specific boostPlatform
    private float speed;
    private float boostDistance;
    private float smoothTime;

    // References for components that are used for boosting
    private Rigidbody2D rigidBody2D;

    // Vectors for handling boosts speed, direction and distance that gameObject is able to travel
    private Vector2 boostDirection;
    private Vector2 lastDistanceBeforeLeaving;
    private Vector2 smoothedMovementInput;
    private Vector2 movementInputSmoothVelocity;

    //
    private bool objectStillInside = false;


    //
    // Instantiate what needs to be instantiated
    void Awake()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
        boostDirection = new Vector2(0,0);
    }

    //
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canBoost)
        {
            smoothedMovementInput = Vector2.SmoothDamp(
                smoothedMovementInput,
                boostDirection,
                ref movementInputSmoothVelocity,
                smoothTime);
            rigidBody2D.velocity = smoothedMovementInput * speed * Time.deltaTime;

            //
            if(objectStillInside)
            {
                lastDistanceBeforeLeaving = transform.position;
            }
            
            // Check if gameObject has passed the distance of boost
            // and if yes -> stop boost and give control back to player
            if (Vector2.Distance(lastDistanceBeforeLeaving, transform.position) >= boostDistance)
            {
                canBoost = false;
                GetComponent<Movement>().enabled = true;
            }
        }
    }

    //
    // Execute boost based on values obtained from specific BoostPlatform
    public void SetUpDash(float speed, float distance, float time, Vector2 direction)
    {
        // Dissable movement, so player won't be able to stop boosting or change boosts direction
        GetComponent<Movement>().enabled = false;

        // Obtain boostDirection (Rotation of platform) and store value of position before boost
        boostDirection = direction;

        // Get values for attribues of boosting from boostplatform
        // every platform can have different boost attributes
        this.speed = speed;
        boostDistance = distance;
        smoothTime = time;
        canBoost = true;

        //
        objectStillInside = true;
    }

    //
    // Update lastDistanceBeforeLeaving for travelling precisely dashDistance from platform
    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("BoostPlatform") || other.CompareTag("WalkalatorPlatform"))
        {
            lastDistanceBeforeLeaving = transform.position;
            objectStillInside = false;
            
            Ryker r = GetComponent<Ryker>();
            r.setIsInBoost(false);
            r.ShowDashDirection();
        }
    }
}
