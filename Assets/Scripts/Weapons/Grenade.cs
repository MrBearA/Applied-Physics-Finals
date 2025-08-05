using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float fuseTime = 3f;
    [SerializeField] private float explosionForce = 700f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float upwardModifier = 1f;

    private bool isArmed;

    public void Arm()
    {
        if (isArmed) return;
        isArmed = true;
        StartCoroutine(FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        yield return new WaitForSeconds(fuseTime);
        Explode();
    }

    private void Explode()
    {
        var hits = Physics.OverlapSphere(transform.position, explosionRadius);
        var seen = new HashSet<RagdollController>();
        foreach (var hit in hits)
        {
            var rc = hit.GetComponentInParent<RagdollController>();
            if (rc != null && seen.Add(rc))
            {
                rc.ActivateRagdoll();
                rc.ApplyExplosionForce(explosionForce, transform.position, explosionRadius, upwardModifier);
            }
        }
        Destroy(gameObject);
    }
}
