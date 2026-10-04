using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoveringSoulBreakerCustomizableCanon : MonoBehaviour
{
    private GameObject currentProjectile;

    public void Fire() {
        currentProjectile = GetComponent<ProjectileFactory>().CraftProjectile();
        currentProjectile.transform.SetParent(transform);
        currentProjectile.GetComponent<Projectile>().SetIsCoveringTrue();
        currentProjectile.GetComponent<Projectile>().Activate();
    }

    public void ClearCurrentProjectile() {
        currentProjectile = null;
    }
}
