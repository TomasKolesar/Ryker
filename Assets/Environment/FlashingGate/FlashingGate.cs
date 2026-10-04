using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingGate : MonoBehaviour
{
    private SpriteRenderer rend;
    private Collider2D coll;
    private TimeFlow timeFlow;

    private float framesPassed;

    private bool angelInDash = false;
    public void setAngelInDash(bool value) {
        angelInDash = value;
    }

    [SerializeField] private float frameDurationForEnabled = 20f;
    
    [Tooltip("The time plyaer has to dash through")]
    [SerializeField] private float frameDurationForDisabled = 30f;

    private void Awake() {
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        timeFlow = FindObjectOfType<TimeFlow>();
        framesPassed = frameDurationForEnabled + frameDurationForDisabled;
    }

    void FixedUpdate()
    {
        // skip any operation when time is frozen (first few frames of dash)
        if(timeFlow.isTimeFrozen()) return;

        // no implementation for normal time needed, because the fg is supposed to stay still

        // blink as often as hell (every frame)
        if (timeFlow.isTimeAngel()) {
            // blink fast to show player that sth changed
            rend.enabled = !rend.enabled;
            if (!coll.enabled && !angelInDash) coll.enabled = true; // #bugfix cause gate used to have deactivated collider after first dash through
        }

        // gives player longer window to make a dash and then prevents him from doing it in shorter one
        if(timeFlow.isTimeSlowmo()) {
            // fg is active
            if (framesPassed > frameDurationForDisabled) {
                rend.enabled = true;
                coll.enabled = true;
            // fg is inactive
            }else if (framesPassed > 0 && framesPassed <= frameDurationForDisabled) {
                rend.enabled = false;
                coll.enabled = false;
            }
        }

        framesPassed--;
        if (framesPassed <= 0) {
            framesPassed = frameDurationForEnabled + frameDurationForDisabled;
        }
    }

    //
    // For collisions of Angel when player's playing for him
    public void DisableIsTrigger()
    {
        if (coll == null)
        {
            coll = GetComponent<Collider2D>();
        }
        coll.isTrigger = false;
    }
}