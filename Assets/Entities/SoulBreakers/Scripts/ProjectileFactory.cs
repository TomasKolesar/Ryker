using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    [SerializeField] private GameObject projectileType; // GameObject with Sprite, collider and tag assigned
    [SerializeField] private float projectileSpeed = 100f;

    [Header("Choose navigaiton type (only one)")]
    [SerializeField] private bool guided;
    [SerializeField] private bool unguided;

    [Header("Choose abilities (as many as you wish)")]
    [SerializeField] private bool destroyable;
    [SerializeField] private bool shiftable;
    [SerializeField] private bool harmful;

    [Header("Only for TypeTripple or TypeQuintuple")]
    [SerializeField] private float spreadRate = 6f;

    [Header("Visuals of abilities")]
    [SerializeField] private GameObject abilityGuidedVisual;
    [SerializeField] private GameObject abilityDestroyableVisual;
    [SerializeField] private Shader abilityShiftableShader;
    [SerializeField] private Sprite projectileBulletHarmfulSprite;
    [SerializeField] private Sprite projectileWideBeamHarmfulSprite;

    [Header("For visuals of avoiding projectiles with dash")]
    [SerializeField] private Material projectileMaterial;

    public GameObject CraftProjectile() {
        // Instantiate type
        GameObject projectileInstance = Instantiate(projectileType, transform.position, Quaternion.identity);
        projectileInstance.AddComponent<Projectile>();

        // Set up for TypeTripple or TypeQuintuple
        bool isTrippleOrQuintuple = false;

        if(projectileInstance.GetComponent<TypeTripple>() != null) {
            projectileInstance.GetComponent<TypeTripple>().SetSpreadRate(spreadRate);
            isTrippleOrQuintuple = true;
        } else if(projectileInstance.GetComponent<TypeQuintuple>() != null) {
            projectileInstance.GetComponent<TypeQuintuple>().SetSpreadRate(spreadRate);
            isTrippleOrQuintuple = true;
        }

        if (isTrippleOrQuintuple) {
            GameObject visualsParental;
            foreach (Transform child in projectileInstance.transform) {

                visualsParental = child.Find("VisualsParental").gameObject;

                if (unguided) {
                    child.gameObject.AddComponent<AbilityGuidedUnguided>();
                } else if (guided) {
                    child.gameObject.AddComponent<AbilityGuidedUnguided>();
                    child.gameObject.GetComponent<AbilityGuidedUnguided>().SetGuidedTrue();
                    GameObject agv = Instantiate(abilityGuidedVisual, transform.position, Quaternion.identity);
                    agv.transform.SetParent(child);
                }
                child.GetComponent<AbilityGuidedUnguided>().SetIsTypeTrippleOrQuintupleTrue();

                if (destroyable) {
                    child.gameObject.AddComponent<AbilityDestroyable>();
                    GameObject adv = Instantiate(abilityDestroyableVisual, transform.position, Quaternion.identity);
                    adv.transform.SetParent(child);
                }
                
                if (shiftable) {
                    child.gameObject.AddComponent<AbilityShiftable>();
                    // Change shader for glowing
                    child.gameObject.GetComponent<SpriteRenderer>().material.shader = abilityShiftableShader;
                }
            }

            if (harmful) {
                projectileInstance.GetComponent<Projectile>().SetIsHarmful();
                projectileInstance.GetComponent<Projectile>().ReplaceSprites(projectileBulletHarmfulSprite);
            }

            projectileInstance.GetComponent<Projectile>().SetSpeed(projectileSpeed);
            
            return projectileInstance;
        }

        // Set up for others (muuuuuuch simpler)
        GameObject vp = projectileInstance.transform.Find("VisualsParental").gameObject;

        if (unguided) {
            projectileInstance.AddComponent<AbilityGuidedUnguided>();
        }
        if (guided) {
            projectileInstance.AddComponent<AbilityGuidedUnguided>();
            projectileInstance.GetComponent<AbilityGuidedUnguided>().SetGuidedTrue();
            GameObject agv = Instantiate(abilityGuidedVisual, transform.position, Quaternion.identity);
            agv.transform.SetParent(vp.transform);
            agv.transform.localPosition = Vector3.zero;
        }
        if (destroyable) {
            projectileInstance.AddComponent<AbilityDestroyable>();
            GameObject adv = Instantiate(abilityDestroyableVisual, transform.position, Quaternion.identity);
            adv.transform.SetParent(vp.transform);
            adv.transform.localPosition = Vector3.zero;
        }
        if (shiftable) {
            projectileInstance.AddComponent<AbilityShiftable>();
            // Change shader for glowing
            projectileInstance.GetComponent<SpriteRenderer>().material.shader = abilityShiftableShader;
        }
        if(harmful) {
            projectileInstance.GetComponent<Projectile>().SetIsHarmful();
            if (projectileInstance.GetComponent<TypeWideBeam>() != null) {
                // For TypeWideBeam
                projectileInstance.GetComponent<Projectile>().ReplaceSprites(projectileWideBeamHarmfulSprite);
            } else {
                // For TypeBasic
                projectileInstance.GetComponent<Projectile>().ReplaceSprites(projectileBulletHarmfulSprite);
            }            
        }

        projectileInstance.GetComponent<Projectile>().SetSpeed(projectileSpeed);

        return projectileInstance;
    }
}
