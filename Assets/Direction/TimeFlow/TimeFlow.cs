using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeFlow : MonoBehaviour
{
    [Header("Current timeflow")]
    [SerializeField] private float timeFlow;

    //
    // Values that other script access and change timeflow variable with setter based on player's actions
    [Header("Timeflow values")]
    [SerializeField] private float normalTime = 1f;
    [SerializeField] private float angelTime = .03f;
    [SerializeField] private float slowmoTime = .01f;
    private float timeFlowBeforeFreeze;

    //
    // Getters
    public float getNormalTime()
    {
        return normalTime;
    }

    public float getAngelTime()
    {
        return angelTime;
    }

    public float getSlowmoTime()
    {
        return slowmoTime;
    }

    // Setter
    public void setTimeFlow(float newTime)
    {
        timeFlow = newTime;
    }

    public void freezeTime()
    {
        if (timeFlowBeforeFreeze != 0) timeFlowBeforeFreeze = timeFlow;
        timeFlow = 0;
    }

    public void unfreezeTime()
    {
        timeFlow = timeFlowBeforeFreeze;
    }

    // Checkers
    public bool isTimeFrozen()
    {
        return timeFlow == 0;
    }

    public bool isTimeNormal()
    {
        return timeFlow == normalTime;
    }

    public bool isTimeAngel()
    {
        return timeFlow == angelTime;
    }

    public bool isTimeSlowmo()
    {
        return timeFlow == slowmoTime;
    }

    //
    // Initial value of timeflow is normal time because every level start with player playing for Ryker
    private void Awake() {
        timeFlow = normalTime;
    }

    //
    // Returned value is used in calculations of movement or other action in other scripts
    public float getTimeFlow()
    {
        return timeFlow;
    }
}
