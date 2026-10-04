using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Elevator : MonoBehaviour
{
    private GameObject rykerReference;
    private bool isMovingTowardsCentre = false;
    [SerializeField] private float movingSpeed = 0.1f;

    private void Update() {
        if (isMovingTowardsCentre) {
            rykerReference.transform.position = Vector2.MoveTowards(    rykerReference.transform.position, 
                                                                        transform.position, 
                                                                        movingSpeed);
            if (rykerReference.transform.position == transform.position) {
                isMovingTowardsCentre = false;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ryker")) {
            rykerReference = other.gameObject;
            rykerReference.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            rykerReference.GetComponent<Movement>().enabled = false;
            FindObjectOfType<PlayerControls>().EnteringElevator();
            isMovingTowardsCentre = true;
        }
    }
}
