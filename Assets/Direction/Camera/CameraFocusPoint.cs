using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFocusPoint : MonoBehaviour
{
    [Tooltip("For one room levels")]
    [SerializeField] private bool stayStill = false;

    [SerializeField] GameObject ryker;
    private GameObject Angel;

    
    private bool isZoomingToAngel = false;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private float cameraLensSizeForAngel = 8.5f;
    [SerializeField] private float cameraZoomSpeed = 1.75f;

    private bool playerControllingAngel = false;
    [SerializeField] float maxCameraSpreadFromAngel = 3.5f;

    //
    // Setters
    public void SetAngel(GameObject angel){
        Angel = angel;
    }

    public void SetPlayerControllingAngel(bool value){
        playerControllingAngel = value;
    }

    public void StartZoomingToAngel(){
        isZoomingToAngel = true;
    }

    //
    // Update position based on who's player controlling
    void Update()
    {
        if (stayStill) return;

        if (isZoomingToAngel){
            cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(
                cinemachineVirtualCamera.m_Lens.OrthographicSize,
                cameraLensSizeForAngel,
                cameraZoomSpeed * Time.deltaTime
            );
            if(cinemachineVirtualCamera.m_Lens.OrthographicSize < cameraLensSizeForAngel + 0.1f){
                isZoomingToAngel = false;
            }
        }

        if (playerControllingAngel){
            // place itself between Angel and mouse cursor, but not more far than maxcameraspreadfromangel from Ryker
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 angelPos = Angel.transform.position;
            Vector3 direction = (angelPos - mousePos) / 2;
            float distance = direction.magnitude;
            if(distance > maxCameraSpreadFromAngel){
                direction = direction.normalized * maxCameraSpreadFromAngel;
            }
            transform.position = angelPos - direction;
        }
    }

    public void StopFollowingAngel(){
        playerControllingAngel = false;
    }
}
