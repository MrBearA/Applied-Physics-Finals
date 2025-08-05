using UnityEngine;
using System.Collections;
using System.Collections.Generic;   

[RequireComponent(typeof(Animator))]
public class RagdollController : MonoBehaviour
{
    [Header("Stand-Up Settings")]
    [Tooltip("Seconds to wait after pelvis touches the ground")]
    [SerializeField] private float standUpDelay = 2f;

    [Tooltip("Pelvis Rigidbody (assign in Inspector)")]
    [SerializeField] private Rigidbody pelvisRigidbody;

    [Tooltip("Name of your StandUp animation state")]
    [SerializeField] private string standUpStateName = "StandUp";

    [Tooltip("Name of your Idle animation state")]
    [SerializeField] private string idleStateName = "Idle";

    // internal lists of all the child bones
    private Animator animator;
    private List<Rigidbody> ragdollBodies = new List<Rigidbody>();
    private List<Collider> ragdollCols = new List<Collider>();
    private bool isRagdolled;

    void Awake()
    {
        // cache animator
        animator = GetComponent<Animator>();

        // gather every child Rigidbody (skip the root)
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
            if (rb != GetComponent<Rigidbody>())
                ragdollBodies.Add(rb);

        // gather every child Collider (skip the root)
        foreach (var col in GetComponentsInChildren<Collider>())
            if (col != GetComponent<Collider>())
                ragdollCols.Add(col);

        // start out in animated (non-ragdoll) mode
        DeactivateRagdoll();
    }

    public void ActivateRagdoll()
    {
        if (isRagdolled) return;
        isRagdolled = true;

        animator.enabled = false;
        foreach (var rb in ragdollBodies) rb.isKinematic = false;
        foreach (var col in ragdollCols) if (!col.isTrigger) col.enabled = true;

        StartCoroutine(WaitForGroundThenStand());
    }

    public void DeactivateRagdoll()
    {
        isRagdolled = false;

        foreach (var rb in ragdollBodies) rb.isKinematic = true;
        foreach (var col in ragdollCols) if (!col.isTrigger) col.enabled = false;

        animator.enabled = true;
        animator.Play(idleStateName);
    }


    public void ApplyExplosionForce(float force, Vector3 origin, float radius, float upModifier)
    {
        foreach (var rb in ragdollBodies)
        {
            if (!rb.isKinematic)
                rb.AddExplosionForce(force, origin, radius, upModifier, ForceMode.Impulse);
        }
    }

    private IEnumerator WaitForGroundThenStand()
    {
        if (pelvisRigidbody == null)
        {
            Debug.LogWarning("[RagdollController] Pelvis Rigidbody not assigned – skipping stand-up.");
            yield break;
        }

        // 1) Raycast down from pelvis to get ground height
        if (!Physics.Raycast(pelvisRigidbody.position, Vector3.down, out RaycastHit hit))
        {
            Debug.LogWarning("[RagdollController] Couldn't find ground under pelvis – skipping stand-up.");
            yield break;
        }
        float groundY = hit.point.y;

        // 2) Wait until the pelvis has fallen near that Y
        while (pelvisRigidbody.position.y > groundY + 0.1f)
            yield return null;

        // 3) Then wait your extra stand-up delay
        yield return new WaitForSeconds(standUpDelay);

        // 4) Snap bones back to kinematic & play stand-up animation
        foreach (var rb in ragdollBodies) rb.isKinematic = true;
        foreach (var col in ragdollCols) if (!col.isTrigger) col.enabled = false;

        animator.enabled = true;
        animator.Play(standUpStateName);

        isRagdolled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // if not ragdolled yet and hit by bullet, grenade, ball or player → ragdoll
        if (!isRagdolled)
        {
            var t = collision.collider.tag;
            if (t == "Bullet" || t == "Grenade" || t == "Ball" ||
               (collision.rigidbody != null && collision.rigidbody.CompareTag("Player")))
            {
                ActivateRagdoll();
            }
        }
    }
}
