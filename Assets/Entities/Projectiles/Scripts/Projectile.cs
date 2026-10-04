using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Custom tagging system")]
    [SerializeField] private List<string> tags = new List<string>();

    private bool leftSoulBreakersCollider = false;

    public void SetIsHarmful() { tags.Add("Harmful"); }
    public bool IsHarmful() { return tags.Contains("Harmful"); }

    //
    // Setters

    public void SetLeftSoulBreakersCollider(bool val) { leftSoulBreakersCollider = val; }

    public void SetSpeed(float speed) {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<AbilityGuidedUnguided>().SetSpeed(speed);
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<AbilityGuidedUnguided>().SetSpeed(speed);
            }
        }
    }

    public void SetTarget(GameObject target) {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<AbilityGuidedUnguided>().SetTarget(target);
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<AbilityGuidedUnguided>().SetTarget(target);
            }
        }
    }

    public void SetIsCoveringTrue() {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<AbilityGuidedUnguided>().SetIsCoveringTrue();
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<AbilityGuidedUnguided>().SetIsCoveringTrue();
            }
        }
    }

    // Basically moves every projectile to the same distance from SB as Ryker is -> show player that he's got hit
    public void SetPlayerDiedAndRykersDistance(bool val, float distance) {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<AbilityGuidedUnguided>().SetPlayerDiedAndRykersDistance(val, distance);
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<AbilityGuidedUnguided>().SetPlayerDiedAndRykersDistance(val, distance);
            }
        }
    }

    //
    // Getters

    public bool GetLeftSoulBreakersCollider() { return leftSoulBreakersCollider; }

    public Vector3[] GetPositionsOnNextFrame() {
        Vector3[] positionsOnNextFrame = null;
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            positionsOnNextFrame = new Vector3[1];
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            positionsOnNextFrame = new Vector3[transform.childCount];
        }

        // TypeBasic and TypeWideBeam
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            positionsOnNextFrame[0] = GetComponent<AbilityGuidedUnguided>().GetPositionOnNextFrame();
            return positionsOnNextFrame;
        }
        
        // TypeTripple and TypeQuintuple
        for (int i = 0; i < transform.childCount; i++) {
            var ability = transform.GetChild(i).GetComponent<AbilityGuidedUnguided>();
            if (ability != null) {
                positionsOnNextFrame[i] = ability.GetPositionOnNextFrame();
            }
        }
        return positionsOnNextFrame;
    }

    public void Activate() {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<AbilityGuidedUnguided>().Activate();
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<AbilityGuidedUnguided>().Activate();
            }
        }
    }

    public void DisableAllColliders() {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<Collider2D>().enabled = false;
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    public void ReplaceSprites(Sprite sprite) {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<SpriteRenderer>().sprite = sprite;
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
    }

    public void DissolveAllProjectiles(Shader disolveShader, float disolveSpeed) {
        if (GetComponent<AbilityGuidedUnguided>() != null) {
            GetComponent<SpriteRenderer>().material.shader = disolveShader;
            GetComponent<AbilityGuidedUnguided>().Dissolve(disolveSpeed);
        } else if (GetComponent<TypeTripple>() != null || GetComponent<TypeQuintuple>() != null) {
            foreach (Transform child in transform) {
                child.gameObject.GetComponent<SpriteRenderer>().material.shader = disolveShader;
                child.gameObject.GetComponent<AbilityGuidedUnguided>().Dissolve(disolveSpeed);
            }
        }
    }
}
